using GcodeParser;
using GcodeParser.GcodeInterpreter.Interpreter;
using Infrastructure.Interfaces.UserSettingService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ControllerService.GCode
{
    public static class GcodeStateFinder
    {
        public static GFrame[] GetPreviousCodes(NCFile nCFile, int lineNumber, IUserSettingsService userSettingsService)
        {
            var currentFrame = GetCurrentFrameGCode(nCFile ?? throw new ArgumentNullException(nameof(nCFile)), lineNumber);
            GFrame[] finalFrame;
            if (currentFrame.GCodes.Contains(2) || currentFrame.GCodes.Contains(3))
                finalFrame = SearchForArcInterpolation(currentFrame);
            else
                finalFrame = SearchForLinearInterpolation(nCFile, currentFrame, lineNumber - 1);
            return GenerateSafetyJog(finalFrame, userSettingsService ?? throw new ArgumentNullException(nameof(userSettingsService)));
        }

        private static GFrame[] GenerateSafetyJog(GFrame[] finalFrame, IUserSettingsService userSettingsService)
        {
            var zMoving =
                new GFrame
                {
                    Coordinate = new AxisCoordinates { Z = userSettingsService.SafetyZCoordinate }
                };
            var xMoving =
                new GFrame
                {
                    Coordinate = new AxisCoordinates { X = userSettingsService.SafetyZCoordinate }
                };
            var yMoving =
                new GFrame
                {
                    Coordinate = new AxisCoordinates { Y = userSettingsService.SafetyZCoordinate }
                };
            zMoving.GCodes.Add(0);
            xMoving.GCodes.Add(0);
            yMoving.GCodes.Add(0);
            var safetyJog = new List<GFrame>
            {
                //1 frame -Z moving
                 zMoving,
                //2 frame -X moving
                xMoving,
                //3 frame -Y moving
                yMoving
            };
            foreach (var frame in finalFrame)
            {
                safetyJog.Add(frame);
            }
            return safetyJog.ToArray();
        }
        private static GFrame[] SearchForLinearInterpolation(NCFile nCFile, GFrame currentFrame, int lineNumber)
        {
            var result = new List<GFrame>();

            CoordintatesExpression coordintatesExpression = new CoordintatesExpression();
            GCodeExpression gCodeExpression = new GCodeExpression();
            MCodeExpression mCodeExpression = new MCodeExpression();
            TCodeExpression tCodeExpression = new TCodeExpression();
            FeedrateExpression feedrateExpression = new FeedrateExpression();

            Context lastLine = new Context { OutputData = new GFrame { Coordinate = new AxisCoordinates() } };
            lastLine.InputString = nCFile.GetClearSomeString(lineNumber, 1);
            coordintatesExpression.Interpret(lastLine);
            if (currentFrame.Coordinate.X != lastLine.OutputData.Coordinate.X)
            {
                var lastX = FindLastCoordinate(nCFile, currentFrame.Coordinate.X, lineNumber - 1, currentFrame.Coordinate.GetType().GetProperties().First(p => p.Name.Contains("X")));
                var xMoving = new GFrame { Coordinate = new AxisCoordinates { X = lastX } };
                xMoving.GCodes.Add(0);
                result.Add(xMoving);
            }
            if (currentFrame.Coordinate.Y != lastLine.OutputData.Coordinate.Y)
            {
                var lastY = FindLastCoordinate(nCFile, currentFrame.Coordinate.Y, lineNumber - 1, currentFrame.Coordinate.GetType().GetProperties().First(p => p.Name.Contains("Y")));
                var yMoving = new GFrame { Coordinate = new AxisCoordinates { Y = lastY } };
                yMoving.GCodes.Add(0);
                result.Add(yMoving);
            }
            if (currentFrame.Coordinate.Z != lastLine.OutputData.Coordinate.Z)
            {
                var lastZ = FindLastCoordinate(nCFile, currentFrame.Coordinate.Z, lineNumber - 1, currentFrame.Coordinate.GetType().GetProperties().First(p => p.Name.Contains("Z")));
                var zMoving = new GFrame { Coordinate = new AxisCoordinates { Z = lastZ } };
                zMoving.GCodes.Add(0);
                result.Add(zMoving);
            }
            currentFrame.Coordinate = lastLine.OutputData.Coordinate;
            result.Add(currentFrame);
            return result.ToArray();
        }
        private static float FindLastCoordinate(NCFile nCFile, float? x, int lineNumber, PropertyInfo coordinateProperty)
        {
            Context result = new Context { OutputData = new GFrame { Coordinate = new AxisCoordinates() } };
            CoordintatesExpression coordintatesExpression = new CoordintatesExpression();
            for (int i = lineNumber; i > 0 && !((float?)coordinateProperty.GetValue(result.OutputData.Coordinate)).HasValue; i--)
            {
                Context context = new Context { InputString = nCFile.GetClearSomeString(i, 1), OutputData = new GFrame { Coordinate = new AxisCoordinates { } } };
                coordintatesExpression.Interpret(context);

                ExcludeOldCoordinates(result.OutputData, context.OutputData);
            }
            var lastCoordinate = (float?)coordinateProperty.GetValue(result.OutputData.Coordinate);
            return lastCoordinate ?? x.Value;
        }
        private static GFrame[] SearchForArcInterpolation(GFrame currentFrame)
        {
            var result = new List<GFrame>();

            if (currentFrame.RValue.HasValue)
            {
                throw new NotImplementedException();
            }
            else
            {
                if (!currentFrame.IValue.HasValue)
                {
                    var xMoving = new GFrame { Coordinate = new AxisCoordinates { X = currentFrame.Coordinate.X } };
                    xMoving.GCodes.Add(0);
                    result.Add(xMoving);

                    currentFrame.Coordinate.X = null;
                    result.Add(currentFrame);
                }
                if (!currentFrame.JValue.HasValue)
                {
                    var yMoving = new GFrame { Coordinate = new AxisCoordinates { Y = currentFrame.Coordinate.Y } };
                    yMoving.GCodes.Add(0);
                    result.Add(yMoving);
                    currentFrame.Coordinate.Y = null;
                    result.Add(currentFrame);
                }
                if (!currentFrame.KValue.HasValue)
                {
                    var zMoving = new GFrame { Coordinate = new AxisCoordinates { Z = currentFrame.Coordinate.Z } };
                    zMoving.GCodes.Add(0);
                    result.Add(zMoving);
                    currentFrame.Coordinate.Z = null;
                    result.Add(currentFrame);
                }
            }

            return result.ToArray();
        }
        private static GFrame GetCurrentFrameGCode(NCFile nCFile, int lineNumber)
        {
            CoordintatesExpression coordintatesExpression = new CoordintatesExpression();
            GCodeExpression gCodeExpression = new GCodeExpression();
            MCodeExpression mCodeExpression = new MCodeExpression();
            TCodeExpression tCodeExpression = new TCodeExpression();
            FeedrateExpression feedrateExpression = new FeedrateExpression();
            Context result = new Context { OutputData = new GFrame { Coordinate = new AxisCoordinates() } };
            for (int i = 0; i < lineNumber; i++)
            {
                Context context = new Context { InputString = nCFile.GetClearSomeString(i, 1), OutputData = new GFrame { Coordinate = new AxisCoordinates { } } };
                gCodeExpression.Interpret(context);
                coordintatesExpression.Interpret(context);
                mCodeExpression.Interpret(context);
                tCodeExpression.Interpret(context);
                feedrateExpression.Interpret(context);

                ExcludeModalCodes(result.OutputData, context.OutputData);
                ExcludeOldCoordinates(result.OutputData, context.OutputData);
                ExcludeOldTCode(result.OutputData, context.OutputData);
                ExcludeOldFeedrate(result.OutputData, context.OutputData);
            }

            return result.OutputData;
        }
        private static void ExcludeOldFeedrate(GFrame outputData1, GFrame outputData2)
        {
            if (outputData2.Feedrate.HasValue)
                outputData1.Feedrate = outputData2.Feedrate;
        }
        private static void ExcludeOldTCode(GFrame outputData1, GFrame outputData2)
        {
            if (outputData2.ToolNumber.HasValue)
                outputData1.ToolNumber = outputData2.ToolNumber;
        }
        private static void ExcludeOldCoordinates(GFrame outputData1, GFrame outputData2)
        {
            if (outputData2.Coordinate.X.HasValue)
            {
                outputData1.Coordinate.X = outputData2.Coordinate.X;
                if (outputData2.IValue.HasValue)
                    outputData1.IValue = outputData2.IValue;
            }
            if (outputData2.Coordinate.Y.HasValue)
            {
                outputData1.Coordinate.Y = outputData2.Coordinate.Y;
                if (outputData2.JValue.HasValue)
                    outputData1.JValue = outputData2.JValue;
            }
            if (outputData2.Coordinate.Z.HasValue)
            {
                outputData1.Coordinate.Z = outputData2.Coordinate.Z;
                if (outputData2.KValue.HasValue)
                    outputData1.KValue = outputData2.KValue;
            }
            if (outputData2.RValue.HasValue)
                outputData1.RValue = outputData2.RValue;
        }
        private static void ExcludeModalCodes(GFrame outputData1, GFrame outputData2)
        {
            foreach (var code in outputData2.GCodes)
            {
                if (GCodeExpression.MutuallyExclusiveCodes.Any(c => c.Contains(code)))
                {
                    outputData1.GCodes.Clear();
                    outputData1.GCodes.UnionWith(outputData1.GCodes.Except(GCodeExpression.MutuallyExclusiveCodes.Single(c => c.Contains(code))));
                    outputData1.GCodes.Add(code);
                }
            }
            foreach (var code in outputData2.MCodes)
            {
                if (MCodeExpression.MutuallyExclusiveCodes.Any(c => c.Contains(code)))
                {
                    outputData1.MCodes.Clear();
                    outputData1.MCodes.UnionWith(outputData1.MCodes.Except(MCodeExpression.MutuallyExclusiveCodes.Single(c => c.Contains(code))));
                    outputData1.MCodes.Add(code);
                }
            }
        }
    }
}
