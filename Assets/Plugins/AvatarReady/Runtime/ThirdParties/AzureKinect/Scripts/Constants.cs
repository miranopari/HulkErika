/*  MIT License
    Copyright (c) Microsoft Corporation. All rights reserved.

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE */

#if AZUREKINECT
namespace kinect
{
    class Constants
    {
        public const float Invalid2DCoordinate = -1;

        public const int InvalidCalculationWindow = -1;

        public const ulong InvalidBodySelectionIndex = ulong.MaxValue;

        public class Validation
        {
            public class ErrorMessages
            {
                public const string InitialStateStability = "Please stand upright in the beginning of the jump";
                public const string EndingStateStability = "Please stand upright in the end of the jump";

                public const string MovementDisplacementHorizontal = "Oops, you need to land where you started";
                public const string MovementDisplacementVertical = "Please stand upright during the jump";

                public const string HandsDisplacement = "Please keep hands on hips while you jump";

                public const string AngleDisplacement = "Please stand upright in the beginning of the jump";

                public const string MaximalHeight = "Please jump again";

                public const string NotEnoughPoints = "Jump does not have sufficient number of points";
            }
        }

        public class TextColor
        {
            public CustomColors green = new CustomColors(0.2f, 0.2f, 0.2f);
            public CustomColors yellow = new CustomColors(0.2f, 0.2f, 0.2f);
        }
    }
}
#endif