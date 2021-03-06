﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simcorp.Laboratory.First
{
    public abstract class Screen
    {
        public string ScreenSize { get; }
        public string ScreenResolution { get; }
        public Screen()
        {
            ScreenSize = "4.7\"";
            ScreenResolution = "1334x750(pixels)";
        }
        public abstract string ToString();
    }
    public abstract class TouchScreen : Screen
    {
        public override string ToString()
        {
            return $" touch screen which has screen diagonal {ScreenSize} with resolution {ScreenResolution}. ";
        }
    }
    public class SingleTouchScreen : TouchScreen
    {
        public override string ToString()
        {
            return "Single" + base.ToString();
        }
    }
    public class MultiTouchScreen : TouchScreen
    {
        public override string ToString()
        {
            return "Multi" + base.ToString();
        }
    }
}
