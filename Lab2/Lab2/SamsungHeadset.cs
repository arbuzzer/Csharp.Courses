﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2
{
    public class SamsungHeadset : IPlayback
    {
        public void Play(object data) {
            Console.WriteLine($"{nameof(SamsungHeadset)} sound");
        }
    }
}