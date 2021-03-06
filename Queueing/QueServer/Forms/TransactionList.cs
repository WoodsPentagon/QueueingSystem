﻿using QueDatabaseModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QueServer
{
    public partial class TransactionList : Form
    {
        public TransactionList()
        {
            InitializeComponent();
        }
        public event EventHandler OnChangesMade;
        private void TransactionList_Load(object sender, EventArgs e)
        {
            LoadValues();
        }
        void LoadValues()
        {
            transactionsTable.Rows.Clear();

            using (var q = new QueeuingEntities())
            {
                foreach (var i in q.Transactions)
                {
                    transactionsTable.Rows.Add(i.Id,
                                               i.Name,
                                               i.Prefix,
                                               i.Details,
                                               "Delete");
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (var c = new CreateTransaction())
            {
                c.OnSave += (a, b) =>
                {
                    changesMade = true;
                    LoadValues();
                };
                c.ShowDialog();
            }
        }
        bool changesMade = false;
        private void transactionsTable_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 4)
            {
                if (MessageBox.Show("Are you sure want to remove this type of transaction?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                    return;
                ///get the transaction id in transaction table
                var tId = (int)transactionsTable.SelectedCells[0].Value;

                using (var q = new QueeuingEntities())
                {
                    ///get the reference of transaction in the model
                    var t = q.Transactions.FirstOrDefault(x => x.Id == tId);
                    ///if found in model, then remove
                    if (t != null)
                    {
                        q.Transactions.Remove(t);
                        q.SaveChanges();
                    }
                }
                ///remove the entry in the table
                transactionsTable.Rows.RemoveAt(e.RowIndex);
                changesMade = true;
            }
        }

        private void TransactionList_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (changesMade)
                OnChangesMade?.Invoke(this, null);
        }
    }
}
