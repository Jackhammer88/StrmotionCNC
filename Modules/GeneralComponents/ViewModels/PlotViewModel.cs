using CNCDraw.Draw;
using CncMachine.Machines;
using GcodeParser.GcodeInterpreter.Interpreter;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Assimp;
using HelixToolkit.SharpDX.Core.Model;
using HelixToolkit.SharpDX.Core.Model.Scene;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Model;
using Infrastructure.Abstract;
using Infrastructure.AggregatorEvents;
using Infrastructure.Enums;
using Infrastructure.Interfaces.CNCControllerService;
using Infrastructure.Interfaces.UserSettingService;
using Infrastructure.Resources.Strings;
using Prism.Commands;
using Prism.Events;
using Prism.Logging;
using SharpDX;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Threading;

namespace GeneralComponents.ViewModels
{
    using Point3D = System.Windows.Media.Media3D.Point3D;
    using Vector3D = System.Windows.Media.Media3D.Vector3D;

    public class PlotViewModel : ViewModelBase
    {
        private HelixToolkit.Wpf.SharpDX.Camera _camera;
        private EffectsManager _effectsManager;
        private HelixToolkit.SharpDX.Core.Geometry3D _linesGeometry;
        private HelixToolkit.SharpDX.Core.Geometry3D _rapidGeometry;
        private Point3D _pointHit;
        private string _frameText;
        private readonly IControllerInformation _controllerInformation;
        private readonly IUserSettingsService _userSettingsService;
        private readonly ILoggerFacade _loggerFacade;
        private readonly IEventAggregator _eventAggregator;
        private readonly IProgramLoader _programLoader;
        private bool isRotating;
        private bool _programLoaded;
        private HelixToolkit.SharpDX.Core.Geometry3D _currentGeometry;
        private int _rotateAngle;
        private bool _showRapid;
        private bool _showGreen;
        private bool _showOrange;
        private bool _dry;
        SceneNodeGroupModel3D _laserModel;
        private string _positionSubtitle;

        public PlotViewModel(IControllerInformation controllerInformation, IUserSettingsService userSettingsService,
                ILoggerFacade loggerFacade, IEventAggregator eventAggregator, IProgramLoader programLoader)
        {
            _userSettingsService = userSettingsService;
            _controllerInformation = controllerInformation;
            _loggerFacade = loggerFacade;
            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
            _programLoader = programLoader ?? throw new ArgumentNullException(nameof(programLoader));

            Title = GeneralComponentsStrings.Plot;

            ShowRapid = true;
            ShowGreen = true;
            ShowOrange = false;

            LaserModel = new SceneNodeGroupModel3D();

            MakeDefaultGeometry(ref _currentGeometry);
            MakeDefaultGeometry(ref _rapidGeometry);
            MakeDefaultGeometry(ref _linesGeometry);
            LoadLaserModel();

            Machine = BuildMachine();
            Machine.FrameChanged += MachineFrameChanged;
            _eventAggregator.GetEvent<CncProgramLoadedEvent>().Subscribe(ProgramLoadedEventExecuted);
            _eventAggregator.GetEvent<ResetEvent>().Subscribe(MachineReseted);
            _programLoader.PropertyChanged += ProgramLoader_PropertyChanged;

            ResetCameraCommand = new DelegateCommand(ResetCameraExecute);
            PortCamera = new HelixToolkit.Wpf.SharpDX.PerspectiveCamera();
            PortEffectsManager = new DefaultEffectsManager();
            ResetCameraCommand.Execute();

            FrameText = "Test";
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Не перехватывать исключения общих типов", Justification = "<Ожидание>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Ликвидировать объекты перед потерей области", Justification = "<Ожидание>")]
        private void LoadLaserModel()
        {
            try
            {
                var loader = new Importer();
                var scene = loader.Load(@"Models3d\ss.obj");
                LaserModel.AddNode(scene.Root);

                Task.Run(() =>
                {
                    while(true)
                    {
                        Application.Current?.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            if (_programLoader.IsProgramRunning)
                                RotateAngle = RotateAngle <= -360 ? 0 : RotateAngle - 10;
                        }), System.Windows.Threading.DispatcherPriority.Normal);
                        Task.Delay(20).Wait();
                    }
                });

                //var loader = new Importer();
                //var scene = loader.Load(@"Models3d\soplo.stl");

                //LoadScene(scene, Colors.Orange.ToColor4(), LaserModel);
                //scene = loader.Load(@"Models3d\ray.stl");
                //LoadScene(scene, new Color4(255, 0, 0, 1), LaserModel);
            }
            catch (Exception ex)
            {
                _loggerFacade.Log(ex.Message, Category.Exception, Priority.High);
            }
        }

        private static void LoadScene(HelixToolkitScene loadedScene, Color4 color4, SceneNodeGroupModel3D loadedGroupModel3D)
        {
            if (loadedScene != null)
            {
                if (loadedScene.Root != null)
                {
                    foreach (var node in loadedScene.Root.Traverse())
                    {
                        if (node is MaterialGeometryNode m)
                        {
                            m.Material = new PhongMaterial(new PhongMaterialCore()) { EmissiveColor = color4 };
                        }
                    }
                }
                loadedGroupModel3D.AddNode(loadedScene.Root);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Не перехватывать исключения общих типов", Justification = "<Ожидание>")]
        private async void ProgramLoadedEventExecuted(string path)
        {
            if (ProgramLoaded)
            {
                LinesGeometry.Positions.Clear();
                LinesGeometry.Indices.Clear();
                RapidGeometry.Positions.Clear();
                RapidGeometry.Indices.Clear();
                CurrentGeometry.Positions.Clear();
                CurrentGeometry.Indices.Clear();
                UpdateGeometry(LinesGeometry);
                UpdateGeometry(RapidGeometry);
                UpdateGeometry(CurrentGeometry);
            }

            try
            {
                FrameText = new FileInfo(path)?.Name;
                _dry = true;
                ShowOrange = false;
                try
                {
                    await Machine.LoadProgramAsync(path).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _loggerFacade.Log(ex.ToString(), Category.Exception, Priority.High);
                    _loggerFacade.Log(GeneralComponentsStrings.PlotBadFileFormat, Category.Warn, Priority.High);
                    return;
                }
                Machine.NextFrame();
                ProgramLoaded = true;

                await Task.Run(() => Machine.Rewind(Machine.Program.Count)).ConfigureAwait(false);

                UpdateGeometry(LinesGeometry);
                UpdateGeometry(RapidGeometry);
                UpdateGeometry(CurrentGeometry);

                _dry = false;
                ShowOrange = true;
                Machine.Rewind(0);


                await Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    PortEffectsManager.Dispose();
                    PortEffectsManager = new DefaultEffectsManager();
                }), System.Windows.Threading.DispatcherPriority.Normal);

                var middlePoints = FindMiddlePoints();
                var test = new Vector3D(0, 0, -(middlePoints.X > middlePoints.Y ? middlePoints.X : middlePoints.Y));

                await Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    PortCamera.LookDirection = test.Z >= 0 ? new Vector3D(0, 0, -0.005) : test;
                    PortCamera.LookAt(middlePoints, 1000);
                }), System.Windows.Threading.DispatcherPriority.Normal);
            }
            catch (Exception exception)
            {
                _loggerFacade.Log($"{exception.Message};{exception.StackTrace}", Category.Exception, Priority.High);
            }
        }
        private Point3D FindMiddlePoints()
        {
            var maxX = LinesGeometry.Positions.Max(p => p.X);
            var maxY = LinesGeometry.Positions.Max(p => p.Y);
            var maxZ = LinesGeometry.Positions.Max(p => p.Z);
            var minX = LinesGeometry.Positions.Min(p => p.X);
            var minY = LinesGeometry.Positions.Min(p => p.Y);
            var minZ = LinesGeometry.Positions.Min(p => p.Z);
            return new Point3D((maxX + minX) / 2, (maxY + minY) / 2, (maxZ + minZ) / 2);
        }
        private void ProgramLoader_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e == null) throw new ArgumentNullException(nameof(e));
            if (e.PropertyName.Equals(nameof(_programLoader.ProgramStringNumber), StringComparison.Ordinal))
            {
                if (_programLoader.ProgramStringNumber == 0)
                {
                    ResetCurrentLines();
                    Machine.Rewind(0);
                }
                else
                    Machine.SetFrame(_programLoader.ProgramStringNumber);
            }
            //if (_programLoader.CurrentState == ProgramLoaderState.Auto && e.PropertyName.Equals(nameof(_programLoader.ProgramStringNumber), StringComparison.Ordinal) && _programLoader.ProgramStringNumber > 2)
            //{
            //    Machine.NextFrame();
            //}
        }

        private void ResetCurrentLines()
        {
            CurrentGeometry = MakeNewLinesGeometry();
        }

        private HelixToolkit.SharpDX.Core.Geometry3D MakeNewLinesGeometry()
        {
            var builder = new LineBuilder();
            builder.AddLine(new Vector3(), new Vector3());
            return builder.ToLineGeometry3D();
        }

        private static void UpdateGeometry(HelixToolkit.SharpDX.Core.Geometry3D geometry)
        {
            geometry.UpdateVertices();
            geometry.UpdateTriangles();
            geometry.UpdateBounds();
        }
        private void MachineReseted()
        {
            MakeDefaultGeometry(ref _currentGeometry);
            MakeDefaultGeometry(ref _rapidGeometry);
            MakeDefaultGeometry(ref _linesGeometry);
            RaisePropertyChanged(nameof(CurrentGeometry));
            RaisePropertyChanged(nameof(RapidGeometry));
            RaisePropertyChanged(nameof(LinesGeometry));
            PortEffectsManager.Dispose();
            PortEffectsManager = new DefaultEffectsManager();
            FrameText = "Reseted";
            ProgramLoaded = false;
        }
        private void MachineFrameChanged(object sender, FrameChangedEventArgs e)
        {
            if (!Machine.CurrentCoordinates.NotEmpty()) return;
            var gModalGroup1 = Machine.ModalGCodes.Single(c => GCodeExpression.MutuallyExclusiveCodes[0].Contains(c));

            var oldCoordinates = new Point3D(Machine.OldCoordinates.X.Value, Machine.OldCoordinates.Y.Value, Machine.OldCoordinates.Z.Value);
            var centers = new ArcCenters { I = Machine.CurrentFrame.IValue, J = Machine.CurrentFrame.JValue, K = Machine.CurrentFrame.KValue };
            bool absolute = !Machine.ModalGCodes.Contains(91);
            var newCoordinates = new Point3D(CalculateNewCoordinate(Machine.CurrentCoordinates.X, oldCoordinates.X, absolute),
                CalculateNewCoordinate(Machine.CurrentCoordinates.Y, oldCoordinates.Y, absolute),
                CalculateNewCoordinate(Machine.CurrentCoordinates.Z, oldCoordinates.Z, absolute));
            var radius = Machine.CurrentFrame.RValue;
            ScaleCoordinates(_userSettingsService.ScaleFactor, ref oldCoordinates, ref newCoordinates, ref centers, ref radius);

            if (_dry)
                DrawDry(gModalGroup1, oldCoordinates, newCoordinates, radius, centers);
            else
                Draw(gModalGroup1, oldCoordinates, newCoordinates, radius, centers);
        }

        private void Draw(float gModalGroup1, Point3D oldCoordinates, Point3D newCoordinates, float? radius, ArcCenters centers)
        {
            ArcInterpolation interpolation;

            switch (gModalGroup1)
            {
                case 0:
                    //Rapid
                    AddLine(oldCoordinates.ToVector3(), newCoordinates.ToVector3(), RapidGeometry);
                    break;
                case 1:
                    //Linear
                    AddLine(oldCoordinates.ToVector3(), newCoordinates.ToVector3(), CurrentGeometry);
                    break;
                case 2:
                    //Arc clockwise
                    if (Machine.CurrentFrame.RValue.HasValue) interpolation = new ArcInterpolation(oldCoordinates, radius: radius.Value, newCoordinates, true);
                    else interpolation = new ArcInterpolation(oldCoordinates, centers, newCoordinates, true);

                    for (int i = 0; i <= 100; i += 5)
                    {
                        var point = interpolation.GetArcCoordinatesEx(i);
                        AddLine(oldCoordinates.ToVector3(), point.ToVector3(), CurrentGeometry);
                        oldCoordinates = point;
                    }
                    AddLine(oldCoordinates.ToVector3(), newCoordinates.ToVector3(), CurrentGeometry);
                    break;
                case 3:
                    //Arc Counter-clockwise
                    if (Machine.CurrentFrame.RValue.HasValue)
                        interpolation = new ArcInterpolation(oldCoordinates, radius: radius.Value, newCoordinates, false);
                    else
                        interpolation = new ArcInterpolation(oldCoordinates, centers, newCoordinates, false);
                    for (int i = 100; i >= 0; i -= 5)
                    {
                        var point = interpolation.GetArcCoordinatesEx(i);
                        AddLine(oldCoordinates.ToVector3(), point.ToVector3(), CurrentGeometry);
                        oldCoordinates = point;
                    }
                    AddLine(oldCoordinates.ToVector3(), newCoordinates.ToVector3(), CurrentGeometry);
                    break;
                default:
                    break;
                    //throw new Exception("Неизвестный код 1-й группы.");
            }


            UpdateCurrentGeometry(CurrentGeometry);
            var programLineNumber = _programLoader.ProgramStringNumber <= 1 ? 0 : _programLoader.ProgramStringNumber;
            if (programLineNumber == 0)
                FrameText = $"{programLineNumber}: {Machine.Program[0]}";
            else
                FrameText = $"{programLineNumber}: {Machine.Program[_programLoader.ProgramStringNumber - 2 < Machine.Program.Count ? _programLoader.ProgramStringNumber - 2 : 0]}";
            if (AxisX != null && AxisY != null)
                PositionSubtitle = $"X{AxisX.PlotPosition} Y{AxisY.PlotPosition}";
        }
        private void DrawDry(float gModalGroup1, Point3D oldCoordinates, Point3D newCoordinates, float? radius, ArcCenters centers)
        {
            ArcInterpolation interpolation;

            switch (gModalGroup1)
            {
                case 0:
                    //Rapid
                    AddLine(oldCoordinates.ToVector3(), newCoordinates.ToVector3(), RapidGeometry);
                    break;
                case 1:
                    //Linear
                    AddLine(oldCoordinates.ToVector3(), newCoordinates.ToVector3(), LinesGeometry);
                    break;
                case 2:
                    //Arc clockwise
                    if (Machine.CurrentFrame.RValue.HasValue) interpolation = new ArcInterpolation(oldCoordinates, radius: radius.Value, newCoordinates, true);
                    else interpolation = new ArcInterpolation(oldCoordinates, centers, newCoordinates, true);

                    for (int i = 0; i <= 100; i += 5)
                    {
                        var point = interpolation.GetArcCoordinatesEx(i);
                        AddLine(oldCoordinates.ToVector3(), point.ToVector3(), LinesGeometry);
                        oldCoordinates = point;
                    }
                    AddLine(oldCoordinates.ToVector3(), newCoordinates.ToVector3(), LinesGeometry);
                    break;
                case 3:
                    //Arc Counter-clockwise
                    if (Machine.CurrentFrame.RValue.HasValue)
                        interpolation = new ArcInterpolation(oldCoordinates, radius: radius.Value, newCoordinates, false);
                    else
                        interpolation = new ArcInterpolation(oldCoordinates, centers, newCoordinates, false);
                    for (int i = 100; i >= 0; i -= 5)
                    {
                        var point = interpolation.GetArcCoordinatesEx(i);
                        AddLine(oldCoordinates.ToVector3(), point.ToVector3(), LinesGeometry);
                        oldCoordinates = point;
                    }
                    AddLine(oldCoordinates.ToVector3(), newCoordinates.ToVector3(), LinesGeometry);
                    break;
                default:
                    break;
                    //throw new Exception("Неизвестный код 1-й группы.");
            }

            FrameText = $"{Machine.FrameNumber}: {Machine}";
        }

        private double CalculateNewCoordinate(float? newCoordinate, double? oldCoordinate, bool absolute)
        {
            return absolute ? (double)(newCoordinate ?? oldCoordinate) : (double)(newCoordinate.HasValue ? newCoordinate + oldCoordinate : oldCoordinate);
        }

        private void UpdateCurrentGeometry(HelixToolkit.SharpDX.Core.Geometry3D geometry)
        {
            geometry.UpdateColors();
            geometry.UpdateVertices();
            geometry.UpdateOctree();
            geometry.UpdateTriangles();
            geometry.UpdateBounds();
        }
        private void AddLine(Vector3 old, Vector3 next, HelixToolkit.SharpDX.Core.Geometry3D geometry)
        {
            int i0 = geometry.Positions.Count;
            geometry.Positions.Add(old);
            geometry.Positions.Add(next);
            geometry.Indices.Add(i0);
            geometry.Indices.Add(i0 + 1);
        }
        private void ScaleCoordinates(float scale, ref Point3D oldCoordinates, ref Point3D newCoordinates, ref ArcCenters centers, ref float? radius)
        {
            oldCoordinates.X /= scale;
            oldCoordinates.Y /= scale;
            oldCoordinates.Z /= scale;
            newCoordinates.X /= scale;
            newCoordinates.Y /= scale;
            newCoordinates.Z /= scale;
            centers.I /= scale;
            centers.J /= scale;
            centers.K /= scale;
            radius /= scale;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Не передавать литералы в качестве локализованных параметров", Justification = "<Ожидание>")]
        private MachineBase BuildMachine()
        {
            MachineBase result;
            switch ((MachineType)_userSettingsService.MachineType)
            {
                case MachineType.Laser:
                    result = new LaserMachine();
                    break;
                case MachineType.Milling:
                    result = new MillMachine();
                    break;
                case MachineType.Turning:
                    result = new LatheMachine();
                    break;
                default:
                    throw new InvalidOperationException("Передан несуществующий тип станка.");
            }
            return result;
        }
        private void ResetCameraExecute()
        {
            var camera = PortCamera as HelixToolkit.Wpf.SharpDX.PerspectiveCamera;
            camera.UpDirection = new Vector3D(0, 1, 0);
            camera.Position = new Point3D(6.22055959701538, -13.7830362319946, 5.71189069747925);
            camera.LookDirection = new Vector3D(-0.200658559799194, 16.1116924285889, -4.40380811691284);
            camera.FieldOfView = 90;
            camera.FarPlaneDistance = 1000;
        }
        private static void MakeDefaultGeometry(ref HelixToolkit.SharpDX.Core.Geometry3D geometry3D)
        {
            var builder = new LineBuilder();
            geometry3D = builder.ToLineGeometry3D();
        }

        public HelixToolkit.Wpf.SharpDX.Camera PortCamera
        {
            get => _camera;
            set => SetProperty(ref _camera, value);
        }
        public EffectsManager PortEffectsManager
        {
            get => _effectsManager;
            set => SetProperty(ref _effectsManager, value);
        }
        public HelixToolkit.SharpDX.Core.Geometry3D LinesGeometry
        {
            get => _linesGeometry;
            set => SetProperty(ref _linesGeometry, value);
        }
        public HelixToolkit.SharpDX.Core.Geometry3D RapidGeometry
        {
            get => _rapidGeometry;
            private set => SetProperty(ref _rapidGeometry, value);
        }
        public HelixToolkit.SharpDX.Core.Geometry3D CurrentGeometry
        {
            get => _currentGeometry;
            set => SetProperty(ref _currentGeometry, value);
        }
        public SceneNodeGroupModel3D LaserModel
        {
            get => _laserModel;
            set => SetProperty(ref _laserModel, value);
        }

        public Point3D PointHit
        {
            get => _pointHit;
            set => SetProperty(ref _pointHit, value);
        }
        public string FrameText
        {
            get => _frameText;
            set => SetProperty(ref _frameText, value);
        }
        public string PositionSubtitle
        {
            get => _positionSubtitle;
            private set => SetProperty(ref _positionSubtitle, value);
        }
        public MachineBase Machine { get; }
        public bool ProgramLoaded
        {
            get => _programLoaded;
            set => SetProperty(ref _programLoaded, value);
        }
        public IMotor AxisX
        {
            get => _controllerInformation.Motors.FirstOrDefault(m => m.Letter.Equals("X", StringComparison.Ordinal));
        }
        public IMotor AxisY
        {
            get => _controllerInformation.Motors.FirstOrDefault(m => m.Letter.Equals("Y", StringComparison.Ordinal));
        }
        public IMotor AxisZ
        {
            get => _controllerInformation.Motors.FirstOrDefault(m => m.Letter.Equals("Z", StringComparison.Ordinal));
        }

        public int RotateAngle
        {
            get => _rotateAngle;
            set => SetProperty(ref _rotateAngle, value);
        }


        public bool ShowRapid
        {
            get => _showRapid;
            set => SetProperty(ref _showRapid, value);
        }
        public bool ShowGreen
        {
            get => _showGreen;
            set => SetProperty(ref _showGreen, value);
        }
        public bool ShowOrange
        {
            get => _showOrange;
            set => SetProperty(ref _showOrange, value);
        }

        public DelegateCommand ResetCameraCommand { get; }
    }
}
