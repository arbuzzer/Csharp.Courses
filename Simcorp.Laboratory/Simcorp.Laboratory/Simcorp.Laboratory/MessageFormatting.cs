﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Simcorp.Laboratory.MobileFeatures;
using Simcorp.Laboratory.Properties;
using static System.Windows.Forms.ListView;
using static System.Windows.Forms.ListViewItem;

namespace Simcorp.Laboratory {
    internal partial class MessageFormatting : Form {
        private readonly SimCorpMobile SimCorpMobile;
        private Func<Message, string[]> TextFormatter;
        private delegate void DischargingDelegate(int value);
        private string UserFilterName;
        private bool MessagingEnabled;

        public MessageFormatting() {
            InitializeComponent();
            DateTimePickerTo.Value = DateTime.Now;
            DateTimePickerTo.MaxDate = DateTime.Today;
            DateTimePickerFrom.MaxDate = DateTime.Today;
            SimCorpMobile = new SimCorpMobile(new MultiTouchScreen(), new LithiumIon(), new SingleModule(), new Stereo());
            SimCorpMobile.Battery.BatteryState = ChargingProgressBar.Value;
            AttachHandlers();
        }

        private int Value {
            get { return SimCorpMobile.Battery.BatteryState; }
            set {
                if (value > ChargingProgressBar.Maximum || value < ChargingProgressBar.Minimum) { return; }
                SimCorpMobile.Battery.BatteryState = value;
                OnValueChanged(SimCorpMobile.Battery.BatteryState);
            }
        }

        private void AttachHandlers() {
            DetachHandlers();
            SimCorpMobile.MessageAddedDelegate = OnMessageAdded;
            MessageComboBox.SelectionChangeCommitted += OnSelected;
            SubscribersComboBox.SelectedIndexChanged += OnUserChanged;
        }

        private void DetachHandlers() {
            SimCorpMobile.MessageAddedDelegate = null;
            MessageComboBox.SelectionChangeCommitted -= OnSelected;
            SubscribersComboBox.SelectedIndexChanged -= OnUserChanged;
        }

        private void OnUserChanged(object sender, EventArgs e) {
            if (Disposing) { return; }

            UserFilterName = SubscribersComboBox.SelectedItem.ToString();
        }

        private void SubscribersComboBoxSelectedIndexChanged(object sender, EventArgs e) {
            if (Disposing) { return; }

            MessageComboBox.Enabled = true;
        }

        private void OnSelected(object sender, EventArgs e) {
            if (Disposing) { return; }

            TextFormatter = MessageFormat.FormatOfMessage[MessageComboBox.SelectedItem.ToString()];
        }

        protected virtual void AddMessage(Message message) {
            MessageListView.Items.Add(new ListViewItem(TextFormatter(message)));
        }

        private void OnMessageAdded(List<Message> messages) {
            if (!MessagingEnabled) { return; }

            if (TextFormatter == null) { return; }

            if (InvokeRequired) {
                Invoke(new SMSProvider.MessageStorageDelegate(OnMessageAdded), messages);
                return;
            }

            foreach (Message message in messages) {
                if (UserFilterName == message.User &&
                    DateTimePickerTo.Value.Date == DateTime.Today) {
                    AddMessage(message);
                }
            }
        }

        private void DateTimePickerToValueChanged(object sender, EventArgs e) {
            if (Disposing) { return; }

            if (DateTimePickerTo.Value.Date != DateTime.Today) {
                MessageListView.Items.Clear();
            }
        }

        private void SearchButtonClick(object sender, EventArgs e) {
            if (Disposing) { return; }

            if (SearchTextBox.Text == string.Empty) {
                MessageBox.Show(Resources.EmptySearchLineWarning);
                return;
            }

            ListViewItemCollection listViewItems = MessageListView.Items;
            IEnumerable<ListViewItem> listOfFoundItems = listViewItems.Cast<ListViewItem>();
            IEnumerable<ListViewItem> foundedItems = listOfFoundItems.Where(item => item.SubItems[0].Text.Contains(SearchTextBox.Text) ||
                                                                                    item.SubItems[1].Text.Contains(SearchTextBox.Text) ||
                                                                                    item.SubItems[2].Text.Contains(SearchTextBox.Text) ||
                                                                                    item.SubItems[3].Text.Contains(SearchTextBox.Text));

            foreach (ListViewItem foundedItem in foundedItems) {
                foundedItem.ForeColor = Color.Red;
            }
        }

        private void FiltrationCheckBoxCheckedChanged(object sender, EventArgs e) {
            if (Disposing) { return; }

            FilterGroupBox.Enabled = FiltrationCheckBox.Checked;
            if (!FiltrationCheckBox.Checked) {
                MessageListView.ForeColor = DefaultForeColor;
            }
        }

        private void MessageComboBoxSelectedIndexChanged(object sender, EventArgs e) {
            if (Disposing) { return; }

            StartThread.Enabled = true;
        }

        private void StartThreadClick(object sender, EventArgs e) {
            if (Disposing) { return; }

            FiltrationCheckBox.Enabled = true;
            SwitchThread(true);
        }

        private void StopThreadClick(object sender, EventArgs e) {
            if (Disposing) { return; }

            SwitchThread(false);
        }

        internal void SwitchThread(bool on) {
            StopThread.Enabled = on;
            StartThread.Enabled = !on;
            MessagingEnabled = on;
        }

        private void StartChargingButtonClick(object sender, EventArgs e) {
            if (Disposing) { return; }

            SwitchCharge(true);
            SimCorpMobile.Battery.BatteryChargingTimer();
            SimCorpMobile.BatteryChargedDelegate = (value) => { Value++; };
        }

        private void StopChargingButtonClick(object sender, EventArgs e) {
            if (Disposing) { return; }

            SwitchCharge(false);
            SimCorpMobile.BatteryChargedDelegate = (value) => { Value--; };
        }

        private void SwitchCharge(bool on) {
            StopCharging.Enabled = on;
            StartCharging.Enabled = !on;
        }

        private void OnValueChanged(int value) {
            if (InvokeRequired) {
                Invoke(new DischargingDelegate(OnValueChanged), value);
                return;
            }

            ChargingProgressBar.Value = value;
        }
    }
}