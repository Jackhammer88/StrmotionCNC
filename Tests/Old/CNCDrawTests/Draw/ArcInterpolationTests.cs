using NUnit.Framework;
using System;
using System.Windows.Media.Media3D;

namespace CNCDraw.Draw.Tests
{
    [TestFixture()]
    [Category("CNCDraw.ArcInterpolation")]
    public class ArcInterpolationTests
    {
        [Test()]
        public void ArcInterpolationTest()
        {
            var cClockwiseInterpolation = new ArcInterpolation(new Point3D(4, 0, 0), new ArcCenters { I = -4, J = 0 }, new Point3D(0, 4, 0), false);
        }

        private void CompareRounded(Point3D first, Point3D second, int afterSeparator)
        {
            Assert.AreEqual(Math.Round(first.X, afterSeparator), Math.Round(second.X, afterSeparator));
            Assert.AreEqual(Math.Round(first.Y, afterSeparator), Math.Round(second.Y, afterSeparator));
            Assert.AreEqual(Math.Round(first.Z, afterSeparator), Math.Round(second.Z, afterSeparator));
        }

        [Test()]
        public void GetArcCoordinatesExIJKTest()
        {
            Assert.Throws<InvalidOperationException>(() => new ArcInterpolation(new Point3D(), new ArcCenters(), new Point3D(), false));
            #region XY
            #region XY: 0-90, R=4; 1:4;0;0 2:0;4;0
            var cClockwiseInterpolation = new ArcInterpolation(new Point3D(4, 0, 0), new ArcCenters { I = -4, J = 0 }, new Point3D(0, 4, 0), false);
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(100), new Point3D(4, 0, 0), 5); // 0 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(99), new Point3D(3.999506529926642394555628510925, 0.06282926924728270301318141323963, 0), 5); //0.9 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(50), new Point3D(2.8284271247461900976033774484194, 2.8284271247461900976033774484194, 0), 5); //45 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(1), new Point3D(0.06282926924728270301318141323963, 3.999506529926642394555628510925, 0), 5); // 89.1 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(0), new Point3D(0, 4, 0), 5); // 90 degrees

            var clockwiseInterpolation = new ArcInterpolation(new Point3D(0, 4, 0), new ArcCenters { I = 0, J = -4 }, new Point3D(4, 0, 0), true);
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(0), new Point3D(0, 4, 0), 5); // 90 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(1), new Point3D(0.06282926924728270301318141323963, 3.999506529926642394555628510925, 0), 5); // 89.1 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(50), new Point3D(2.8284271247461900976033774484194, 2.8284271247461900976033774484194, 0), 5); //45 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(99), new Point3D(3.999506529926642394555628510925, 0.06282926924728270301318141323963, 0), 5); //0.9 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(100), new Point3D(4, 0, 0), 5); // 0 degrees
            #endregion

            # region XY: 90-180, R=4; 1:0;4;0 2:-4;0;0
            cClockwiseInterpolation = new ArcInterpolation(new Point3D(0, 4, 0), new ArcCenters { I = 0, J = -4 }, new Point3D(-4, 0, 0), false);
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(100), new Point3D(0, 4, 0), 5); // 0 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(99), new Point3D(-0.06282926924728270301318141323963, 3.999506529926642394555628510925, 0), 5); //90 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(50), new Point3D(-2.8284271247461900976033774484194, 2.8284271247461900976033774484194, 0), 5); //135 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(1), new Point3D(-3.999506529926642394555628510925, 0.06282926924728270301318141323963, 0), 5); // 179.1 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(0), new Point3D(-4, 0, 0), 5); // 180 degrees

            clockwiseInterpolation = new ArcInterpolation(new Point3D(-4, 0, 0), new ArcCenters { I = 4, J = 0 }, new Point3D(0, 4, 0), true);
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(0), new Point3D(-4, 0, 0), 5); // 180 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(1), new Point3D(-3.999506529926642394555628510925, 0.06282926924728270301318141323963, 0), 5); // 179.1 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(50), new Point3D(-2.8284271247461900976033774484194, 2.8284271247461900976033774484194, 0), 5); //135 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(99), new Point3D(-0.06282926924728270301318141323963, 3.999506529926642394555628510925, 0), 5); //90.9 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(100), new Point3D(0, 4, 0), 5); // 90 degrees
            #endregion

            #region XY: 180-230, R=4; 1:-4;0;0 2:0;-4;0
            cClockwiseInterpolation = new ArcInterpolation(new Point3D(-4, 0, 0), new ArcCenters { I = 4, J = 0 }, new Point3D(0, -4, 0), false);
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(100), new Point3D(-4, 0, 0), 5); // 180 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(99), new Point3D(-3.999506529926642394555628510925, -0.06282926924728270301318141323963, 0), 5); //180.9 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(50), new Point3D(-2.8284271247461900976033774484194, -2.8284271247461900976033774484194, 0), 5); //225 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(1), new Point3D(-0.06282926924728270301318141323963, -3.999506529926642394555628510925, 0), 5); // 229.1 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(0), new Point3D(0, -4, 0), 5); // 230 degrees

            clockwiseInterpolation = new ArcInterpolation(new Point3D(0, -4, 0), new ArcCenters { I = 0, J = 4 }, new Point3D(-4, 0, 0), true);
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(0), new Point3D(0, -4, 0), 5); // 230 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(1), new Point3D(-0.06282926924728270301318141323963, -3.999506529926642394555628510925, 0), 5); // 229.1 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(50), new Point3D(-2.8284271247461900976033774484194, -2.8284271247461900976033774484194, 0), 5); //225 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(99), new Point3D(-3.999506529926642394555628510925, -0.06282926924728270301318141323963, 0), 5); //180.9 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(100), new Point3D(-4, 0, 0), 5); // 180 degrees
            #endregion

            #region XY: 230-360, R=4, 1:0;-4;0, 2:4;0;0
            cClockwiseInterpolation = new ArcInterpolation(new Point3D(0, -4, 0), new ArcCenters { I = 0, J = 4 }, new Point3D(4, 0, 0), false);
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(100), new Point3D(0, -4, 0), 5); // 230 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(99), new Point3D(0.06282926924728270301318141323963, -3.999506529926642394555628510925, 0), 5); //230.9 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(50), new Point3D(2.8284271247461900976033774484194, -2.8284271247461900976033774484194, 0), 5); //275 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(1), new Point3D(3.999506529926642394555628510925, -0.06282926924728270301318141323963, 0), 5); // 359.1 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(0), new Point3D(4, 0, 0), 5); // 360 degrees

            clockwiseInterpolation = new ArcInterpolation(new Point3D(4, 0, 0), new ArcCenters { I = -4, J = 0 }, new Point3D(0, -4, 0), true);
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(0), new Point3D(4, 0, 0), 5); // 360 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(1), new Point3D(3.999506529926642394555628510925, -0.06282926924728270301318141323963, 0), 5); // 359.1 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(50), new Point3D(2.8284271247461900976033774484194, -2.8284271247461900976033774484194, 0), 5); //275 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(99), new Point3D(0.06282926924728270301318141323963, -3.999506529926642394555628510925, 0), 5); //230.9 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(100), new Point3D(0, -4, 0), 5); // 230 degrees
            #endregion
            #endregion
            #region XZ
            #region XZ: 0-90, R=4; 1:4;0;0 2:0;0;4
            cClockwiseInterpolation = new ArcInterpolation(new Point3D(4, 0, 0), new ArcCenters { I = -4, K = 0 }, new Point3D(0, 0, 4), false);
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(100), new Point3D(4, 0, 0), 5); // 0 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(99), new Point3D(3.999506529926642394555628510925, 0, 0.06282926924728270301318141323963), 5); //0.9 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(50), new Point3D(2.8284271247461900976033774484194, 0, 2.8284271247461900976033774484194), 5); //45 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(1), new Point3D(0.06282926924728270301318141323963, 0, 3.999506529926642394555628510925), 5); // 89.1 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(0), new Point3D(0, 0, 4), 5); // 90 degrees

            clockwiseInterpolation = new ArcInterpolation(new Point3D(0, 0, 4), new ArcCenters { I = 0, K = -4 }, new Point3D(4, 0, 0), true);
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(0), new Point3D(0, 0, 4), 5); // 90 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(1), new Point3D(0.06282926924728270301318141323963, 0, 3.999506529926642394555628510925), 5); // 89.1 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(50), new Point3D(2.8284271247461900976033774484194, 0, 2.8284271247461900976033774484194), 5); //45 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(99), new Point3D(3.999506529926642394555628510925, 0, 0.06282926924728270301318141323963), 5); //0.9 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(100), new Point3D(4, 0, 0), 5); // 0 degrees
            #endregion

            # region XZ: 90-180, R=4; 1:0;0;4 2:-4;0;0
            cClockwiseInterpolation = new ArcInterpolation(new Point3D(0, 0, 4), new ArcCenters { I = 0, K = -4 }, new Point3D(-4, 0, 0), false);
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(100), new Point3D(0, 0, 4), 5); // 0 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(99), new Point3D(-0.06282926924728270301318141323963, 0, 3.999506529926642394555628510925), 5); //90 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(50), new Point3D(-2.8284271247461900976033774484194, 0, 2.8284271247461900976033774484194), 5); //135 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(1), new Point3D(-3.999506529926642394555628510925, 0, 0.06282926924728270301318141323963), 5); // 179.1 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(0), new Point3D(-4, 0, 0), 5); // 180 degrees

            clockwiseInterpolation = new ArcInterpolation(new Point3D(-4, 0, 0), new ArcCenters { I = 4, K = 0 }, new Point3D(0, 0, 4), true);
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(0), new Point3D(-4, 0, 0), 5); // 180 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(1), new Point3D(-3.999506529926642394555628510925, 0, 0.06282926924728270301318141323963), 5); // 179.1 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(50), new Point3D(-2.8284271247461900976033774484194, 0, 2.8284271247461900976033774484194), 5); //135 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(99), new Point3D(-0.06282926924728270301318141323963, 0, 3.999506529926642394555628510925), 5); //90.9 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(100), new Point3D(0, 0, 4), 5); // 90 degrees
            #endregion

            #region XZ: 180-230, R=4; 1:-4;0;0 2:0;0;-4
            cClockwiseInterpolation = new ArcInterpolation(new Point3D(-4, 0, 0), new ArcCenters { I = 4, K = 0 }, new Point3D(0, 0, -4), false);
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(100), new Point3D(-4, 0, 0), 5); // 180 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(99), new Point3D(-3.999506529926642394555628510925, 0, -0.06282926924728270301318141323963), 5); //180.9 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(50), new Point3D(-2.8284271247461900976033774484194, 0, -2.8284271247461900976033774484194), 5); //225 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(1), new Point3D(-0.06282926924728270301318141323963, 0, -3.999506529926642394555628510925), 5); // 229.1 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(0), new Point3D(0, 0, -4), 5); // 230 degrees

            clockwiseInterpolation = new ArcInterpolation(new Point3D(0, 0, -4), new ArcCenters { I = 0, K = 4 }, new Point3D(-4, 0, 0), true);
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(0), new Point3D(0, 0, -4), 5); // 230 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(1), new Point3D(-0.06282926924728270301318141323963, 0, -3.999506529926642394555628510925), 5); // 229.1 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(50), new Point3D(-2.8284271247461900976033774484194, 0, -2.8284271247461900976033774484194), 5); //225 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(99), new Point3D(-3.999506529926642394555628510925, 0, -0.06282926924728270301318141323963), 5); //180.9 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(100), new Point3D(-4, 0, 0), 5); // 180 degrees
            #endregion

            #region XZ: 230-360, R=4, 1:0;0;-4, 2:4;0;0
            cClockwiseInterpolation = new ArcInterpolation(new Point3D(0, 0, -4), new ArcCenters { I = 0, K = 4 }, new Point3D(4, 0, 0), false);
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(100), new Point3D(0, 0, -4), 5); // 230 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(99), new Point3D(0.06282926924728270301318141323963, 0, -3.999506529926642394555628510925), 5); //230.9 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(50), new Point3D(2.8284271247461900976033774484194, 0, -2.8284271247461900976033774484194), 5); //275 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(1), new Point3D(3.999506529926642394555628510925, 0, -0.06282926924728270301318141323963), 5); // 359.1 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(0), new Point3D(4, 0, 0), 5); // 360 degrees

            clockwiseInterpolation = new ArcInterpolation(new Point3D(4, 0, 0), new ArcCenters { I = -4, K = 0 }, new Point3D(0, 0, -4), true);
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(0), new Point3D(4, 0, 0), 5); // 360 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(1), new Point3D(3.999506529926642394555628510925, 0, -0.06282926924728270301318141323963), 5); // 359.1 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(50), new Point3D(2.8284271247461900976033774484194, 0, -2.8284271247461900976033774484194), 5); //275 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(99), new Point3D(0.06282926924728270301318141323963, 0, -3.999506529926642394555628510925), 5); //230.9 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(100), new Point3D(0, 0, -4), 5); // 230 degrees
            #endregion
            #endregion
            #region YZ
            #region YZ: 0-90, R=4; 1:0;4;0 2:0;0;4
            cClockwiseInterpolation = new ArcInterpolation(new Point3D(0, 4, 0), new ArcCenters { J = -4, K = 0 }, new Point3D(0, 0, 4), false);
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(100), new Point3D(0, 4, 0), 5); // 0 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(99), new Point3D(0, 3.999506529926642394555628510925, 0.06282926924728270301318141323963), 5); //0.9 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(50), new Point3D(0, 2.8284271247461900976033774484194, 2.8284271247461900976033774484194), 5); //45 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(1), new Point3D(0, 0.06282926924728270301318141323963, 3.999506529926642394555628510925), 5); // 89.1 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(0), new Point3D(0, 0, 4), 5); // 90 degrees

            clockwiseInterpolation = new ArcInterpolation(new Point3D(0, 0, 4), new ArcCenters { J = 0, K = -4 }, new Point3D(0, 4, 0), true);
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(0), new Point3D(0, 0, 4), 5); // 90 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(1), new Point3D(0, 0.06282926924728270301318141323963, 3.999506529926642394555628510925), 5); // 89.1 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(50), new Point3D(0, 2.8284271247461900976033774484194, 2.8284271247461900976033774484194), 5); //45 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(99), new Point3D(0, 3.999506529926642394555628510925, 0.06282926924728270301318141323963), 5); //0.9 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(100), new Point3D(0, 4, 0), 5); // 0 degrees
            #endregion

            # region YZ: 90-180, R=4; 1:0;0;4 2:0,-4;0
            cClockwiseInterpolation = new ArcInterpolation(new Point3D(0, 0, 4), new ArcCenters { J = 0, K = -4 }, new Point3D(0, -4, 0), false);
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(100), new Point3D(0, 0, 4), 5); // 0 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(99), new Point3D(0, -0.06282926924728270301318141323963, 3.999506529926642394555628510925), 5); //90 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(50), new Point3D(0, -2.8284271247461900976033774484194, 2.8284271247461900976033774484194), 5); //135 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(1), new Point3D(0, -3.999506529926642394555628510925, 0.06282926924728270301318141323963), 5); // 179.1 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(0), new Point3D(0, -4, 0), 5); // 180 degrees

            clockwiseInterpolation = new ArcInterpolation(new Point3D(0, -4, 0), new ArcCenters { J = 4, K = 0 }, new Point3D(0, 0, 4), true);
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(0), new Point3D(0, -4, 0), 5); // 180 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(1), new Point3D(0, -3.999506529926642394555628510925, 0.06282926924728270301318141323963), 5); // 179.1 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(50), new Point3D(0, -2.8284271247461900976033774484194, 2.8284271247461900976033774484194), 5); //135 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(99), new Point3D(0, -0.06282926924728270301318141323963, 3.999506529926642394555628510925), 5); //90.9 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(100), new Point3D(0, 0, 4), 5); // 90 degrees
            #endregion

            #region YZ: 180-230, R=4; 1:0;-4;0 2:0;0;-4
            cClockwiseInterpolation = new ArcInterpolation(new Point3D(0, -4, 0), new ArcCenters { J = 4, K = 0 }, new Point3D(0, 0, -4), false);
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(100), new Point3D(0, -4, 0), 5); // 180 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(99), new Point3D(0, -3.999506529926642394555628510925, -0.06282926924728270301318141323963), 5); //180.9 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(50), new Point3D(0, -2.8284271247461900976033774484194, -2.8284271247461900976033774484194), 5); //225 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(1), new Point3D(0, -0.06282926924728270301318141323963, -3.999506529926642394555628510925), 5); // 229.1 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(0), new Point3D(0, 0, -4), 5); // 230 degrees

            clockwiseInterpolation = new ArcInterpolation(new Point3D(0, 0, -4), new ArcCenters { J = 0, K = 4 }, new Point3D(0, -4, 0), true);
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(0), new Point3D(0, 0, -4), 5); // 230 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(1), new Point3D(0, -0.06282926924728270301318141323963, -3.999506529926642394555628510925), 5); // 229.1 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(50), new Point3D(0, -2.8284271247461900976033774484194, -2.8284271247461900976033774484194), 5); //225 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(99), new Point3D(0, -3.999506529926642394555628510925, -0.06282926924728270301318141323963), 5); //180.9 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(100), new Point3D(0, -4, 0), 5); // 180 degrees
            #endregion

            #region YZ: 230-360, R=4, 1:0;0;-4, 2:0;4;0
            cClockwiseInterpolation = new ArcInterpolation(new Point3D(0, 0, -4), new ArcCenters { J = 0, K = 4 }, new Point3D(0, 4, 0), false);
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(100), new Point3D(0, 0, -4), 5); // 230 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(99), new Point3D(0, 0.06282926924728270301318141323963, -3.999506529926642394555628510925), 5); //230.9 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(50), new Point3D(0, 2.8284271247461900976033774484194, -2.8284271247461900976033774484194), 5); //275 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(1), new Point3D(0, 3.999506529926642394555628510925, -0.06282926924728270301318141323963), 5); // 359.1 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(0), new Point3D(0, 4, 0), 5); // 360 degrees

            clockwiseInterpolation = new ArcInterpolation(new Point3D(0, 4, 0), new ArcCenters { J = -4, K = 0 }, new Point3D(0, 0, -4), true);
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(0), new Point3D(0, 4, 0), 5); // 360 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(1), new Point3D(0, 3.999506529926642394555628510925, -0.06282926924728270301318141323963), 5); // 359.1 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(50), new Point3D(0, 2.8284271247461900976033774484194, -2.8284271247461900976033774484194), 5); //275 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(99), new Point3D(0, 0.06282926924728270301318141323963, -3.999506529926642394555628510925), 5); //230.9 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(100), new Point3D(0, 0, -4), 5); // 230 degrees
            #endregion
            #endregion
        }

        [Test()]
        public void GetArcCoordinatesExRTest()
        {
            Assert.Throws<InvalidOperationException>(() => new ArcInterpolation(new Point3D(), radius: 4, new Point3D(), true));
            #region XY
            #region XY: 0-90, R=4; 1:4;0;0 2:0;4;0
            var cClockwiseInterpolation = new ArcInterpolation(new Point3D(4, 0, 0), radius: 4, new Point3D(0, 4, 0), false);
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(100), new Point3D(4, 0, 0), 5); // 0 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(99), new Point3D(3.999506529926642394555628510925, 0.06282926924728270301318141323963, 0), 5); //0.9 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(50), new Point3D(2.8284271247461900976033774484194, 2.8284271247461900976033774484194, 0), 5); //45 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(1), new Point3D(0.06282926924728270301318141323963, 3.999506529926642394555628510925, 0), 5); // 89.1 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(0), new Point3D(0, 4, 0), 5); // 90 degrees

            var clockwiseInterpolation = new ArcInterpolation(new Point3D(0, 4, 0), radius: 4, new Point3D(4, 0, 0), true);
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(0), new Point3D(0, 4, 0), 5); // 90 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(1), new Point3D(0.06282926924728270301318141323963, 3.999506529926642394555628510925, 0), 5); // 89.1 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(50), new Point3D(2.8284271247461900976033774484194, 2.8284271247461900976033774484194, 0), 5); //45 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(99), new Point3D(3.999506529926642394555628510925, 0.06282926924728270301318141323963, 0), 5); //0.9 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(100), new Point3D(4, 0, 0), 5); // 0 degrees
            #endregion

            # region XY: 90-180, R=4; 1:0;4;0 2:-4;0;0
            cClockwiseInterpolation = new ArcInterpolation(new Point3D(0, 4, 0), radius: 4, new Point3D(-4, 0, 0), false);
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(100), new Point3D(0, 4, 0), 5); // 0 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(99), new Point3D(-0.06282926924728270301318141323963, 3.999506529926642394555628510925, 0), 5); //90 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(50), new Point3D(-2.8284271247461900976033774484194, 2.8284271247461900976033774484194, 0), 5); //135 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(1), new Point3D(-3.999506529926642394555628510925, 0.06282926924728270301318141323963, 0), 5); // 179.1 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(0), new Point3D(-4, 0, 0), 5); // 180 degrees

            clockwiseInterpolation = new ArcInterpolation(new Point3D(-4, 0, 0), radius: 4, new Point3D(0, 4, 0), true);
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(0), new Point3D(-4, 0, 0), 5); // 180 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(1), new Point3D(-3.999506529926642394555628510925, 0.06282926924728270301318141323963, 0), 5); // 179.1 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(50), new Point3D(-2.8284271247461900976033774484194, 2.8284271247461900976033774484194, 0), 5); //135 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(99), new Point3D(-0.06282926924728270301318141323963, 3.999506529926642394555628510925, 0), 5); //90.9 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(100), new Point3D(0, 4, 0), 5); // 90 degrees
            #endregion

            #region XY: 180-230, R=4; 1:-4;0;0 2:0;-4;0
            cClockwiseInterpolation = new ArcInterpolation(new Point3D(-4, 0, 0), radius: 4, new Point3D(0, -4, 0), false);
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(100), new Point3D(-4, 0, 0), 5); // 180 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(99), new Point3D(-3.999506529926642394555628510925, -0.06282926924728270301318141323963, 0), 5); //180.9 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(50), new Point3D(-2.8284271247461900976033774484194, -2.8284271247461900976033774484194, 0), 5); //225 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(1), new Point3D(-0.06282926924728270301318141323963, -3.999506529926642394555628510925, 0), 5); // 229.1 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(0), new Point3D(0, -4, 0), 5); // 230 degrees

            clockwiseInterpolation = new ArcInterpolation(new Point3D(0, -4, 0), radius: 4, new Point3D(-4, 0, 0), true);
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(0), new Point3D(0, -4, 0), 5); // 230 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(1), new Point3D(-0.06282926924728270301318141323963, -3.999506529926642394555628510925, 0), 5); // 229.1 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(50), new Point3D(-2.8284271247461900976033774484194, -2.8284271247461900976033774484194, 0), 5); //225 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(99), new Point3D(-3.999506529926642394555628510925, -0.06282926924728270301318141323963, 0), 5); //180.9 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(100), new Point3D(-4, 0, 0), 5); // 180 degrees
            #endregion

            #region XY: 230-360, R=4, 1:0;-4;0, 2:4;0;0
            cClockwiseInterpolation = new ArcInterpolation(new Point3D(0, -4, 0), radius: 4, new Point3D(4, 0, 0), false);
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(100), new Point3D(0, -4, 0), 5); // 230 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(99), new Point3D(0.06282926924728270301318141323963, -3.999506529926642394555628510925, 0), 5); //230.9 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(50), new Point3D(2.8284271247461900976033774484194, -2.8284271247461900976033774484194, 0), 5); //275 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(1), new Point3D(3.999506529926642394555628510925, -0.06282926924728270301318141323963, 0), 5); // 359.1 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(0), new Point3D(4, 0, 0), 5); // 360 degrees

            clockwiseInterpolation = new ArcInterpolation(new Point3D(4, 0, 0), radius: 4, new Point3D(0, -4, 0), true);
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(0), new Point3D(4, 0, 0), 5); // 360 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(1), new Point3D(3.999506529926642394555628510925, -0.06282926924728270301318141323963, 0), 5); // 359.1 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(50), new Point3D(2.8284271247461900976033774484194, -2.8284271247461900976033774484194, 0), 5); //275 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(99), new Point3D(0.06282926924728270301318141323963, -3.999506529926642394555628510925, 0), 5); //230.9 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(100), new Point3D(0, -4, 0), 5); // 230 degrees
            #endregion
            #endregion
            #region XZ
            #region XZ: 0-90, R=4; 1:4;0;0 2:0;0;4
            cClockwiseInterpolation = new ArcInterpolation(new Point3D(4, 0, 0), radius: 4, new Point3D(0, 0, 4), false);
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(100), new Point3D(4, 0, 0), 5); // 0 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(99), new Point3D(3.999506529926642394555628510925, 0, 0.06282926924728270301318141323963), 5); //0.9 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(50), new Point3D(2.8284271247461900976033774484194, 0, 2.8284271247461900976033774484194), 5); //45 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(1), new Point3D(0.06282926924728270301318141323963, 0, 3.999506529926642394555628510925), 5); // 89.1 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(0), new Point3D(0, 0, 4), 5); // 90 degrees

            clockwiseInterpolation = new ArcInterpolation(new Point3D(0, 0, 4), radius: 4, new Point3D(4, 0, 0), true);
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(0), new Point3D(0, 0, 4), 5); // 90 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(1), new Point3D(0.06282926924728270301318141323963, 0, 3.999506529926642394555628510925), 5); // 89.1 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(50), new Point3D(2.8284271247461900976033774484194, 0, 2.8284271247461900976033774484194), 5); //45 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(99), new Point3D(3.999506529926642394555628510925, 0, 0.06282926924728270301318141323963), 5); //0.9 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(100), new Point3D(4, 0, 0), 5); // 0 degrees
            #endregion

            # region XZ: 90-180, R=4; 1:0;0;4 2:-4;0;0
            cClockwiseInterpolation = new ArcInterpolation(new Point3D(0, 0, 4), radius: 4, new Point3D(-4, 0, 0), false);
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(100), new Point3D(0, 0, 4), 5); // 0 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(99), new Point3D(-0.06282926924728270301318141323963, 0, 3.999506529926642394555628510925), 5); //90 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(50), new Point3D(-2.8284271247461900976033774484194, 0, 2.8284271247461900976033774484194), 5); //135 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(1), new Point3D(-3.999506529926642394555628510925, 0, 0.06282926924728270301318141323963), 5); // 179.1 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(0), new Point3D(-4, 0, 0), 5); // 180 degrees

            clockwiseInterpolation = new ArcInterpolation(new Point3D(-4, 0, 0), radius: 4, new Point3D(0, 0, 4), true);
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(0), new Point3D(-4, 0, 0), 5); // 180 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(1), new Point3D(-3.999506529926642394555628510925, 0, 0.06282926924728270301318141323963), 5); // 179.1 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(50), new Point3D(-2.8284271247461900976033774484194, 0, 2.8284271247461900976033774484194), 5); //135 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(99), new Point3D(-0.06282926924728270301318141323963, 0, 3.999506529926642394555628510925), 5); //90.9 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(100), new Point3D(0, 0, 4), 5); // 90 degrees
            #endregion

            #region XZ: 180-230, R=4; 1:-4;0;0 2:0;0;-4
            cClockwiseInterpolation = new ArcInterpolation(new Point3D(-4, 0, 0), radius: 4, new Point3D(0, 0, -4), false);
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(100), new Point3D(-4, 0, 0), 5); // 180 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(99), new Point3D(-3.999506529926642394555628510925, 0, -0.06282926924728270301318141323963), 5); //180.9 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(50), new Point3D(-2.8284271247461900976033774484194, 0, -2.8284271247461900976033774484194), 5); //225 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(1), new Point3D(-0.06282926924728270301318141323963, 0, -3.999506529926642394555628510925), 5); // 229.1 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(0), new Point3D(0, 0, -4), 5); // 230 degrees

            clockwiseInterpolation = new ArcInterpolation(new Point3D(0, 0, -4), radius: 4, new Point3D(-4, 0, 0), true);
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(0), new Point3D(0, 0, -4), 5); // 230 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(1), new Point3D(-0.06282926924728270301318141323963, 0, -3.999506529926642394555628510925), 5); // 229.1 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(50), new Point3D(-2.8284271247461900976033774484194, 0, -2.8284271247461900976033774484194), 5); //225 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(99), new Point3D(-3.999506529926642394555628510925, 0, -0.06282926924728270301318141323963), 5); //180.9 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(100), new Point3D(-4, 0, 0), 5); // 180 degrees
            #endregion

            #region XZ: 230-360, R=4, 1:0;0;-4, 2:4;0;0
            cClockwiseInterpolation = new ArcInterpolation(new Point3D(0, 0, -4), radius: 4, new Point3D(4, 0, 0), false);
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(100), new Point3D(0, 0, -4), 5); // 230 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(99), new Point3D(0.06282926924728270301318141323963, 0, -3.999506529926642394555628510925), 5); //230.9 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(50), new Point3D(2.8284271247461900976033774484194, 0, -2.8284271247461900976033774484194), 5); //275 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(1), new Point3D(3.999506529926642394555628510925, 0, -0.06282926924728270301318141323963), 5); // 359.1 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(0), new Point3D(4, 0, 0), 5); // 360 degrees

            clockwiseInterpolation = new ArcInterpolation(new Point3D(4, 0, 0), radius: 4, new Point3D(0, 0, -4), true);
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(0), new Point3D(4, 0, 0), 5); // 360 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(1), new Point3D(3.999506529926642394555628510925, 0, -0.06282926924728270301318141323963), 5); // 359.1 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(50), new Point3D(2.8284271247461900976033774484194, 0, -2.8284271247461900976033774484194), 5); //275 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(99), new Point3D(0.06282926924728270301318141323963, 0, -3.999506529926642394555628510925), 5); //230.9 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(100), new Point3D(0, 0, -4), 5); // 230 degrees
            #endregion
            #endregion
            #region YZ
            #region YZ: 0-90, R=4; 1:0;4;0 2:0;0;4
            cClockwiseInterpolation = new ArcInterpolation(new Point3D(0, 4, 0), radius: 4, new Point3D(0, 0, 4), false);
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(100), new Point3D(0, 4, 0), 5); // 0 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(99), new Point3D(0, 3.999506529926642394555628510925, 0.06282926924728270301318141323963), 5); //0.9 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(50), new Point3D(0, 2.8284271247461900976033774484194, 2.8284271247461900976033774484194), 5); //45 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(1), new Point3D(0, 0.06282926924728270301318141323963, 3.999506529926642394555628510925), 5); // 89.1 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(0), new Point3D(0, 0, 4), 5); // 90 degrees

            clockwiseInterpolation = new ArcInterpolation(new Point3D(0, 0, 4), radius: 4, new Point3D(0, 4, 0), true);
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(0), new Point3D(0, 0, 4), 5); // 90 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(1), new Point3D(0, 0.06282926924728270301318141323963, 3.999506529926642394555628510925), 5); // 89.1 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(50), new Point3D(0, 2.8284271247461900976033774484194, 2.8284271247461900976033774484194), 5); //45 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(99), new Point3D(0, 3.999506529926642394555628510925, 0.06282926924728270301318141323963), 5); //0.9 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(100), new Point3D(0, 4, 0), 5); // 0 degrees
            #endregion

            # region YZ: 90-180, R=4; 1:0;0;4 2:0,-4;0
            cClockwiseInterpolation = new ArcInterpolation(new Point3D(0, 0, 4), radius: 4, new Point3D(0, -4, 0), false);
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(100), new Point3D(0, 0, 4), 5); // 0 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(99), new Point3D(0, -0.06282926924728270301318141323963, 3.999506529926642394555628510925), 5); //90 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(50), new Point3D(0, -2.8284271247461900976033774484194, 2.8284271247461900976033774484194), 5); //135 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(1), new Point3D(0, -3.999506529926642394555628510925, 0.06282926924728270301318141323963), 5); // 179.1 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(0), new Point3D(0, -4, 0), 5); // 180 degrees

            clockwiseInterpolation = new ArcInterpolation(new Point3D(0, -4, 0), radius: 4, new Point3D(0, 0, 4), true);
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(0), new Point3D(0, -4, 0), 5); // 180 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(1), new Point3D(0, -3.999506529926642394555628510925, 0.06282926924728270301318141323963), 5); // 179.1 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(50), new Point3D(0, -2.8284271247461900976033774484194, 2.8284271247461900976033774484194), 5); //135 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(99), new Point3D(0, -0.06282926924728270301318141323963, 3.999506529926642394555628510925), 5); //90.9 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(100), new Point3D(0, 0, 4), 5); // 90 degrees
            #endregion

            #region YZ: 180-230, R=4; 1:0;-4;0 2:0;0;-4
            cClockwiseInterpolation = new ArcInterpolation(new Point3D(0, -4, 0), radius: 4, new Point3D(0, 0, -4), false);
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(100), new Point3D(0, -4, 0), 5); // 180 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(99), new Point3D(0, -3.999506529926642394555628510925, -0.06282926924728270301318141323963), 5); //180.9 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(50), new Point3D(0, -2.8284271247461900976033774484194, -2.8284271247461900976033774484194), 5); //225 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(1), new Point3D(0, -0.06282926924728270301318141323963, -3.999506529926642394555628510925), 5); // 229.1 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(0), new Point3D(0, 0, -4), 5); // 230 degrees

            clockwiseInterpolation = new ArcInterpolation(new Point3D(0, 0, -4), radius: 4, new Point3D(0, -4, 0), true);
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(0), new Point3D(0, 0, -4), 5); // 230 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(1), new Point3D(0, -0.06282926924728270301318141323963, -3.999506529926642394555628510925), 5); // 229.1 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(50), new Point3D(0, -2.8284271247461900976033774484194, -2.8284271247461900976033774484194), 5); //225 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(99), new Point3D(0, -3.999506529926642394555628510925, -0.06282926924728270301318141323963), 5); //180.9 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(100), new Point3D(0, -4, 0), 5); // 180 degrees
            #endregion

            #region YZ: 230-360, R=4, 1:0;0;-4, 2:0;4;0
            cClockwiseInterpolation = new ArcInterpolation(new Point3D(0, 0, -4), radius: 4, new Point3D(0, 4, 0), false);
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(100), new Point3D(0, 0, -4), 5); // 230 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(99), new Point3D(0, 0.06282926924728270301318141323963, -3.999506529926642394555628510925), 5); //230.9 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(50), new Point3D(0, 2.8284271247461900976033774484194, -2.8284271247461900976033774484194), 5); //275 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(1), new Point3D(0, 3.999506529926642394555628510925, -0.06282926924728270301318141323963), 5); // 359.1 degrees
            CompareRounded(cClockwiseInterpolation.GetArcCoordinatesEx(0), new Point3D(0, 4, 0), 5); // 360 degrees

            clockwiseInterpolation = new ArcInterpolation(new Point3D(0, 4, 0), radius: 4, new Point3D(0, 0, -4), true);
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(0), new Point3D(0, 4, 0), 5); // 360 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(1), new Point3D(0, 3.999506529926642394555628510925, -0.06282926924728270301318141323963), 5); // 359.1 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(50), new Point3D(0, 2.8284271247461900976033774484194, -2.8284271247461900976033774484194), 5); //275 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(99), new Point3D(0, 0.06282926924728270301318141323963, -3.999506529926642394555628510925), 5); //230.9 degrees
            CompareRounded(clockwiseInterpolation.GetArcCoordinatesEx(100), new Point3D(0, 0, -4), 5); // 230 degrees
            #endregion
            #endregion
        }
    }
}