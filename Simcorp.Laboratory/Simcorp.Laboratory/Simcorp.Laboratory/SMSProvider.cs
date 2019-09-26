﻿using System;
using System.Collections.Generic;
using System.Threading;

namespace Simcorp.Laboratory {
    internal class SMSProvider {
        public delegate void MessageStorageDelegate(List<Message> messages);
        public event MessageStorageDelegate MessageAdded;

        public SMSProvider() {
            StartMessageThread();
        }

        internal readonly List<Message> Messages = new List<Message> {
            new Message("+380667243917",
                "You have new message from Mother",
                "Thank you so much, my sweet <3",
                DateTime.Now),
            new Message("+380974108621",
                "You have new message from Girlfriend",
                "What time I can come to you today?",
                DateTime.Now),
            new Message("+380504620705",
                "You have new message from Brother",
                "Did you wish mom a Happy Birthday?",
                DateTime.Now)
        };

        private void StartTimer() {
            var autoEvent = new AutoResetEvent(false);
            var startMessage = new Timer(state => RaiseMessageAddedEvent(Messages), autoEvent, 0, 1500);
        }

        private void StartMessageThread() {
            var messageThread = new Thread(StartTimer);
            messageThread.Start();
        }

        private void RaiseMessageAddedEvent(List<Message> messages) {
            MessageAdded?.Invoke(messages);
        }
    }
}