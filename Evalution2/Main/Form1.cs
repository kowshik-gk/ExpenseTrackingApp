﻿using ExpenseTracker.Manager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace ExpenseTracker
{
    public partial class Form1 : Form
    {
        private List<string> categoryList = new List<string>();
        private DataGridViewComboBoxColumn comboBoxobj;
        private List<List<string>> expanseDataList = new List<List<string>>();
        private bool isCategaryAddOn;
        private bool isCategaryUpdateOn;
        private bool isCategaryRemoveOn;
        private bool isDataGridViewRightButton;
        private int currentRowIndex;
        private int expenseId = 0;
        private Control switchBtnSelected;
        private AddCategoryU addCategoryU;
        private CategoryDeleteU categaryDeleteU;
        private CategoryUpdateU categoryUpdateU;
        private List<string> monthCBSource = new List<string>() { "None", "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
        private List<string> yearCBSource = new List<string>() { "None" };
        private List<string> dayCBSource = new List<string>() { "None" };
        private bool isSelectAll;
        public static NotificationThrowManager notificationThrowManager = new NotificationThrowManager();
        private Timer startTimer = new Timer();
        public Form1()
        {
            InitializeComponent();

            typeof(Panel).InvokeMember("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.NonPublic, null, expenseDataGridViewP, new object[] { true });
            typeof(Panel).InvokeMember("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.NonPublic, null, tapViewP, new object[] { true });
            typeof(Panel).InvokeMember("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.NonPublic, null, panel18, new object[] { true });
            typeof(Panel).InvokeMember("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.NonPublic, null, mainP, new object[] { true });
            Load += ExpenseTrackerLoad;
            AddExpenseBtn.Click += AddExpenseBtnClick;
            categoryGenralCB.SelectedIndexChanged += CategoryenralCBSelectedIndexChanged;
            addCategryBtn.Click += AddCategryBtnClick;
            removeCategoryBtn.Click += RemoveCategoryBtnClick;

            dashBoardBtn.Click += DashBoardBtnClick;
            expenseDataGridView.CellClick += DataGridViewCellClick;
            expenseDataGridView.MouseClick += DataGridViewMouseClick;
            editBtn.Click += EditExpenseBtnClick;
            filterBtnDayView.Click += FilterBtnDayViewClick;
            deleteBtn.Click += DeleteAllExpenseBtnClick;
            totalCost.Click += TotalCostClick;
            cutomDateViewFilterBtn.Click += CutomDateViewFilterBtnClick;
            updateCategoryBtn.Click += UpdateCategryBtnClick;
            topP.Resize += TopPResize;
            selectAllBtn.Click += selectAllBtnClick;
            this.KeyDown += DataGridViewKeyDown;
            startTimer.Interval = 1500;
            startTimer.Tick += StartApplication;
            fromDatePicker.ValueChanged += FromDatePickerValueChanged;
            toDatePicker.ValueChanged += ToDatePickerValueChanged;
            this.FormClosed += Form1FormClosed;

        }

        private void Form1FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void ToDatePickerValueChanged(object sender, EventArgs e)
        {
            if (toDatePicker.Value < fromDatePicker.Value)
            {
                toDatePicker.Value = fromDatePicker.Value;
            }
        }

        private void FromDatePickerValueChanged(object sender, EventArgs e)
        {
            if (toDatePicker.Value < fromDatePicker.Value)
                toDatePicker.Value = fromDatePicker.Value;
        }

        private void StartApplication(object sender, EventArgs e)
        {
            mainP.Visible = true;
            startTimer.Stop();
        }

        private void selectAllBtnClick(object sender, EventArgs e)
        {
            if (expenseDataGridViewP.Visible)
            {
                foreach (DataGridViewRow row in expenseDataGridView.Rows)
                {
                    if (row.Visible)
                        row.Selected = true;
                }
            }
        }

        private void DataGridViewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A && (mainTabControl.SelectedTab == customDayViewPage || mainTabControl.SelectedTab == mainTabControl.SelectedTab || mainTabControl.SelectedTab == expenseAddPage))
            {
                foreach (DataGridViewRow row in expenseDataGridView.Rows)
                {
                    if (row.Visible)
                        row.Selected = true;
                }
            }
        }

        private void TopPResize(object sender, EventArgs e)
        {
            expenseMangerLB.Location = new Point(topP.Width / 2 - expenseMangerLB.Width / 2, 15);
        }

        private void CutomDateViewFilterBtnClick(object sender, EventArgs e)
        {

            DateTime fromDate = fromDatePicker.Value;
            DateTime toDate = toDatePicker.Value;

            for (int i = 0; i < expenseDataGridView.RowCount; i++)
            {
                if (fromDate <= ((DateTime)expenseDataGridView.Rows[i].Cells["DateAndTime"].Value) && toDate >= ((DateTime)expenseDataGridView.Rows[i].Cells["DateAndTime"].Value) && (expenseDataGridView.Rows[i].Cells["CategoryID"].Value.ToString() == categoryGenralCB.SelectedValue.ToString() || categoryGenralCB.SelectedValue.ToString() == "1"))
                {

                    expenseDataGridView.Rows[i].Visible = true;
                }
                else
                {
                    CurrencyManager manager = (CurrencyManager)BindingContext[expenseDataGridView.DataSource];
                    manager.SuspendBinding();
                    expenseDataGridView.Rows[i].Visible = false;
                    manager.ResumeBinding();
                }
            }
        }

        private void TotalCostClick(object sender, EventArgs e)
        {
            int totalCost = 0;
            for (int i = 0; i < expenseDataGridView.Rows.Count; i++)
            {
                if (expenseDataGridView.Rows[i].Visible)
                {
                    totalCost += ((Expense)expenseDataGridView.Rows[i].DataBoundItem).Amount;
                }

            }
            totalCostDisplayLB.Text = totalCost + ".00 /-";
        }

        private void RefreshDayViewPageFilter()
        {
            yearCBFilter.SelectedIndex = 0;
            monthCBFilter.SelectedIndex = 0;
            dayCBFilter.SelectedIndex = 0;

            FilterBtnDayViewClick(this, EventArgs.Empty);
        }
        private void FilterBtnDayViewClick(object sender, EventArgs e)
        {


            int year = -1, month = -1, day = -1;
            if ((string)yearCBFilter.SelectedValue != "None" && yearCBFilter.SelectedValue != null)
                year = int.Parse((string)yearCBFilter.SelectedValue);
            if ((string)monthCBFilter.SelectedValue != "None" && monthCBFilter.SelectedValue != null)
                month = int.Parse("" + monthCBFilter.SelectedIndex);
            if ((string)dayCBFilter.SelectedValue != "None" && dayCBFilter.SelectedValue != null)
                day = int.Parse((string)dayCBFilter.SelectedValue);

            bool isYearMatch, isMonthMatch, isDayMatch;

            for (int i = 0; i < expenseDataGridView.RowCount; i++)
            {
                //  var a = expenseDataGridView.Rows[i].Cells["DateAndTime"].Value;          
                // MessageBox.Show(""+a.GetType());
                isYearMatch = isMonthMatch = isDayMatch = false;
                if (year == -1 || year == ((DateTime)expenseDataGridView.Rows[i].Cells["DateAndTime"].Value).Year)
                {
                    isYearMatch = true;
                }
                if (month == -1 || month == ((DateTime)expenseDataGridView.Rows[i].Cells["DateAndTime"].Value).Month)
                {
                    isMonthMatch = true;
                }
                if (day == -1 || day == ((DateTime)expenseDataGridView.Rows[i].Cells["DateAndTime"].Value).Day)
                {
                    isDayMatch = true;
                }
                if (isYearMatch && isMonthMatch && isDayMatch && (categoryGenralCB.SelectedValue.ToString() == expenseDataGridView.Rows[i].Cells["CategoryID"].Value.ToString() || categoryGenralCB.SelectedValue.ToString() == "1"))
                {
                    expenseDataGridView.Rows[i].Visible = true;
                }
                else
                {
                    CurrencyManager manager = (CurrencyManager)BindingContext[expenseDataGridView.DataSource];
                    manager.SuspendBinding();
                    expenseDataGridView.Rows[i].Visible = false;
                    manager.ResumeBinding();
                }
            }

        }

        private void RefreshGenCategory()
        {
            CategoryenralCBSelectedIndexChanged(this, EventArgs.Empty);
        }

        //Add Expense
        private void AddExpenseBtnClick(object sender, EventArgs e)
        {
            if (!isCategaryAddOn )
            {
                AddPage obj = new AddPage();
                obj.OnSubmit += AddExpenseToDataBase;
                obj.ShowDialog();
            }
            else{
                notificationThrowManager.CreateNotification($"Off the add category option\n before add expense", NotificationType.Warning);
            }
        }
        private void AddExpenseToDataBase(object sender, Expense expense)
        {
            try
            {
                ExpenseManager.AddExpense(expense);
                RefreshGenCategory();
                Form obj = (Form)(sender);
                obj.Dispose();
                notificationThrowManager.CreateNotification("Expense added successful", NotificationType.None);
                int categoryExceedAmount = ExpenseManager.GetCurrentMonthLimitExceedAmount(expense.CategoryID);
                if (categoryExceedAmount > 0)
                {
                    notificationThrowManager.CreateNotification($"{ExpenseManager.CategoryDictionary["" + expense.CategoryID].CategoryName} Limit Exceed Over\n{categoryExceedAmount}", NotificationType.Warning);
                }

                CategoryDataGridViewRefresh();
                ExpenseDataGridViewRefresh();
            }
            catch
            {
                notificationThrowManager.CreateNotification("Invalid input value", NotificationType.Error);
            }

        }

        //Update Expense
        private int editExpenseAmountValue;
        private int editExpenseCategoryId;
        private bool iseditincategory = false;
        private void UpdateEditedExpenseInDataBase(object sender, Expense expense)
        {
            try
            {
                ExpenseManager.UpdateExpense(expense);
                Form obj = (Form)sender;
                obj.Dispose();
                RefreshGenCategory();
                notificationThrowManager.CreateNotification("Expense Updated successfully", NotificationType.None);

                //   Expense expenseObj = ExpenseManager.GetExpenseById(int.Parse(expenseDataGridView.Rows[expenseDataGridView.SelectedRows[0].Index].Cells["ID"].Value.ToString()));

                int categoryExceedAmount = ExpenseManager.GetCurrentMonthLimitExceedAmount(expense.CategoryID);
                if (categoryExceedAmount > 0)
                {
                    notificationThrowManager.CreateNotification($"{ExpenseManager.CategoryDictionary["" + expense.CategoryID].CategoryName} Limit Exceed Over\n{categoryExceedAmount}", NotificationType.Warning);
                }
                CategoryDataGridViewRefresh();
                ExpenseDataGridViewRefresh();
                iseditincategory = false;
            }
            catch (Exception e)
            {
                notificationThrowManager.CreateNotification("Invalid input", NotificationType.Error);
                if (iseditincategory)
                    ExpenseManager.EditCurrentMonthUsedAmountToCategory(-editExpenseAmountValue, editExpenseCategoryId);
            }

        }
        private void EditExpenseBtnClick(object sender, EventArgs e)
        {
            if (expenseDataGridView.RowCount > 0 && expenseDataGridView.SelectedRows.Count > 0)
            {
                Expense expense = (Expense)expenseDataGridView.SelectedRows[0].DataBoundItem;

                editExpenseAmountValue = expense.Amount;
                editExpenseCategoryId = expense.CategoryID;
                if (expense.DateAndTime.Month == DateTime.Now.Month && expense.DateAndTime.Year == DateTime.Now.Year)
                {
                    ExpenseManager.EditCurrentMonthUsedAmountToCategory(-editExpenseAmountValue, editExpenseCategoryId);
                    iseditincategory = true;
                }

                AddPage obj = new AddPage(expense);
                obj.OnSubmit += UpdateEditedExpenseInDataBase;
                obj.ShowDialog();
            }
        }

        //Remove Expense 
        private void DeleteAllSelectedExpenseFromDataBase(object sender, bool isDelete)
        {
            if (isDelete)
            {
                for (int i = 0; i < expenseDataGridView.SelectedRows.Count; i++)
                {
                    ExpenseManager.DeleteExpense((Expense)(expenseDataGridView.SelectedRows[i].DataBoundItem));
                }

                CategoryDataGridViewRefresh();
                ExpenseDataGridViewRefresh();
            }

            Form obj = (Form)(sender);
            obj.Dispose();
        }
        private void DeleteAllExpenseBtnClick(object sender, EventArgs e)
        {
            MessageConfirmForm messageBox = new MessageConfirmForm("Do you want to delete  the\n record");
            messageBox.OnClickResult += DeleteAllSelectedExpenseFromDataBase;
            messageBox.ShowDialog();
        }
        private void DeleteOneSelectedExpenseFromDataBase(object sender, bool isYes)
        {
            if (isYes)
            {
                ExpenseManager.DeleteExpense((Expense)expenseDataGridView.Rows[currentRowIndex].DataBoundItem);
                RefreshGenCategory();
                CategoryDataGridViewRefresh();
                ExpenseDataGridViewRefresh();
            }
            Form obj = (Form)(sender);
            obj.Dispose();
        }

        //Add Category
        private void AddCategryBtnClick(object sender, EventArgs e)
        {
            if (!isCategaryRemoveOn && !isCategaryUpdateOn)
            {
                isCategaryAddOn = !isCategaryAddOn;
                if (isCategaryAddOn)
                {
                    addCategoryU = new AddCategoryU();
                    addCategoryU.OnclickSubmit += AddCategory;
                    categoryModifyP.Controls.Add(addCategoryU);
                    addCategoryU.Dock = DockStyle.Right;
                    addCategryBtn.BackColor = Color.FromArgb(210, 210, 210);
                    addCategryBtn.ForeColor = Color.Black;
                }
                else
                {
                    if (addCategoryU != null)
                    {
                        addCategoryU.Dispose();
                        addCategoryU = null;
                    }
                    addCategryBtn.BackColor = Color.Teal;
                    addCategryBtn.ForeColor = Color.White;
                }
            }
        }
        public void AddCategory(object sender, Category category)
        {
            try
            {
                ExpenseManager.AddCategory(category);
                Control control = (Control)sender;
                control.Dispose();
                addCategoryU = null;
                //NotificationSent
                notificationThrowManager.CreateNotification(category.CategoryName + " Category added successfully", NotificationType.None);
                AddCategryBtnClick(this, EventArgs.Empty);
                ComboBoxValueSet();
                CategoryDataGridViewRefresh();
            }
            catch
            {
                notificationThrowManager.CreateNotification("Invalid input", NotificationType.Error);
            }

        }

        //Update Category
        private void UpdateCategryBtnClick(object sender, EventArgs e)
        {
            if (!isCategaryRemoveOn && !isCategaryAddOn)
            {
                isCategaryUpdateOn = !isCategaryUpdateOn;
                if (isCategaryUpdateOn)
                {
                    categoryUpdateU = new CategoryUpdateU();
                    categoryUpdateU.OnClickSave += UpdateCategory;
                    categoryModifyP.Controls.Add(categoryUpdateU);
                    categoryUpdateU.Dock = DockStyle.Right;
                    updateCategoryBtn.BackColor = Color.FromArgb(210, 210, 210);
                    updateCategoryBtn.ForeColor = Color.Black;
                }
                else
                {
                    if (categoryUpdateU != null)
                    {
                        categoryUpdateU.Dispose();
                        categoryUpdateU = null;
                    }
                    updateCategoryBtn.BackColor = Color.Teal;
                    updateCategoryBtn.ForeColor = Color.White;
                }
            }
        }
        public void UpdateCategory(object sender, Category category)
        {
            try
            {
                ExpenseManager.UpdateCategory(category);
                Control control = (Control)sender;
                control.Dispose();
                ComboBoxValueSet();
                UpdateCategryBtnClick(this, EventArgs.Empty);
                notificationThrowManager.CreateNotification(category.CategoryName + " Category Updated successfully", NotificationType.None);
                ComboBoxValueSet();
                ExpenseDataGridViewRefresh();
                CategoryDataGridViewRefresh();


            }
            catch
            {
                notificationThrowManager.CreateNotification("Invalid input", NotificationType.Error);
            }
        }

        //Remove Category
        private void RemoveCategoryBtnClick(object sender, EventArgs e)
        {
            if (!isCategaryAddOn && !isCategaryUpdateOn)
            {
                isCategaryRemoveOn = !isCategaryRemoveOn;
                if (isCategaryRemoveOn)
                {
                    categaryDeleteU = new CategoryDeleteU();
                    categaryDeleteU.onClickRemove += RemoveCategory;
                    categoryModifyP.Controls.Add(categaryDeleteU);
                    categaryDeleteU.Dock = DockStyle.Right;
                    removeCategoryBtn.BackColor = Color.FromArgb(210, 210, 210);
                    removeCategoryBtn.ForeColor = Color.Black;
                }
                else
                {
                    if (categaryDeleteU != null)
                    {
                        categaryDeleteU.Dispose();
                        categaryDeleteU = null;
                    }

                    removeCategoryBtn.BackColor = Color.Teal;
                    removeCategoryBtn.ForeColor = Color.White;
                }
            }
        }
        private void RemoveCategory(object sender, int categoryId)
        {
            try
            {
                string categoryName = ExpenseManager.CategoryDictionary["" + categoryId].CategoryName;
                ExpenseManager.DeleteCategory(categoryId);
                notificationThrowManager.CreateNotification(categoryName + " Category removed successfully", NotificationType.None);
                Control control = (Control)(sender);
                control.Dispose();
                RemoveCategoryBtnClick(this, EventArgs.Empty);
                categaryDeleteU = null;
                ComboBoxValueSet();
                ExpenseDataGridViewRefresh();
                CategoryDataGridViewRefresh();

            }
            catch
            {
                //notificationThrowManager.CreateNotification(ExpenseManager2.FindCategoryName(categoryId) + " Category not exits ", NotificationType.Warning);
            }

        }



        public void ComboBoxValueSet()
        {

            categoryGenralCB.DataSource = null;
            List<Category> categoryComboboxSource = ExpenseManager.CategoryDictionary.Values.ToList();
            for (int i = 0; i < categoryComboboxSource.Count; i++)
            {
                if (categoryComboboxSource[i].Id == ExpenseManager.OtherCategoryId)
                {
                    Category categrory = categoryComboboxSource[i];
                    categoryComboboxSource.RemoveAt(i);
                    categoryComboboxSource.Add(categrory);
                }
            }
            categoryGenralCB.DataSource = ExpenseManager.CategoryDictionary.Values.ToList();
            categoryGenralCB.DisplayMember = "CategoryName";
            categoryGenralCB.ValueMember = "ID";
        }


        private void DataGridViewMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {

                MessageConfirmForm messageBox = new MessageConfirmForm("Do you want to delete  the\n record");
                messageBox.OnClickResult += DeleteAllSelectedExpenseFromDataBase;
                messageBox.ShowDialog();
            }
        }

        private void DataGridViewCellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (expenseDataGridView.RowCount > 0 && isDataGridViewRightButton == true)
            {
                currentRowIndex = e.RowIndex;
                expenseDataGridView.Rows[currentRowIndex].DefaultCellStyle.BackColor = Color.FromArgb(210, 210, 230);
            }

        }


        public int ExpenseBudgetMonthLimitCalculate(string categoryId, DateTime dateTime)
        {

            int total = 0;
            //// DateTime dateTime = DateTime.Now;
            // for(int i=0;i<ExpenseManager.ExpenseList.Count;i++){
            //     if (ExpenseManager.ExpenseList[i].CategoryId == categoryId && ExpenseManager.ExpenseList[i].DateAndTime.Month == dateTime.Month && ExpenseManager.ExpenseList[i].DateAndTime.Year == dateTime.Year)
            //         total += ExpenseManager.ExpenseList[i].Amount;
            // }
            return total;
        }


        private void CategoryenralCBSelectedIndexChanged(object sender, EventArgs e)
        {

            if (categoryGenralCB.SelectedValue != null)
            {
                for (int i = 0; i < expenseDataGridView.Rows.Count; i++)
                {
                    // MessageBox.Show(categoryGenralCB.GetItemText(categoryGenralCB.SelectedItem)+"  "+ expenseDataGridView.Rows[i].Cells[1].Value.ToString());
                    if (expenseDataGridView.Rows[i].Cells["CategoryID"].Value.ToString() == categoryGenralCB.SelectedValue.ToString() || categoryGenralCB.SelectedValue.ToString() == "1")
                    {
                        expenseDataGridView.Rows[i].Visible = true;
                    }
                    else
                    {
                        CurrencyManager currencyManager1 = (CurrencyManager)BindingContext[expenseDataGridView.DataSource];
                        currencyManager1.SuspendBinding();
                        expenseDataGridView.Rows[i].Visible = false;
                        currencyManager1.ResumeBinding();
                    }
                }
            }


        }

        private void ExpenseTrackerLoad(object sender, EventArgs e)
        {
            ExpenseManager.CategoryRefresh();
            ExpenseManager.ExpenseRefresh();

            DataGridviewRefresh();
            ComboBoxValueSet();
            categoryGenralCB.DisplayMember = "CategoryName";
            categoryGenralCB.ValueMember = "Id";
            startTimer.Start();
            TopPResize(this, EventArgs.Empty);


            expenseDataGridView.Columns[0].Visible = false;
            expenseDataGridView.Columns[1].Visible = false;
            categoryDataGridView.Columns[0].Visible = false;
            //expenseDataGridView.RowTemplate.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            //expenseDataGridView.RowTemplate.Height = 50; 

            mainTabControl.ItemSize = new Size(0, 1);
            mainTabControl.SelectedTab = expenseAddPage;
            expensePageBtn.BackColor = Color.FromArgb(230, 230, 230);
            switchBtnSelected = expensePageBtn;

            //Date Combobox
            for (int i = 1950; i <= 2050; i++)
            {
                yearCBSource.Add("" + i);
            }
            for (int i = 1; i <= 31; i++)
            {
                dayCBSource.Add("" + i);
            }
            yearCBFilter.DataSource = yearCBSource;
            dayCBFilter.DataSource = dayCBSource;
            monthCBFilter.DataSource = monthCBSource;


            columsChart.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            columsChart.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
            columsChart.ChartAreas[0].AxisX.MinorGrid.Enabled = false;
            columsChart.ChartAreas[0].AxisY.MinorGrid.Enabled = false;
            columsChart.ChartAreas[0].AxisX.MajorTickMark.Enabled = false;
            columsChart.ChartAreas[0].AxisY.MajorTickMark.Enabled = false;
            columsChart.ChartAreas[0].AxisX.MinorTickMark.Enabled = false;
            columsChart.ChartAreas[0].AxisY.MinorTickMark.Enabled = false;

            RefreshGenCategory();

                                                     
        }
        public int NumSteps(string s)
        {
            int step = 0;
            while (s != "1")
            {
                if (s[s.Length - 1] == '0')
                {
                    s = s.Substring(0,s.Length-1);
                    //for (int i = 0; i < s.Length; i++)
                    //{

                    //    if (s[i] == '0')
                    //    {
                    //        s = s.Substring(0, i) + "1" + ((i + 1 < s.Length) ? s.Substring(i + 1) : "");
                    //        break;
                    //    }

                    //}
                }
                else
                {
                    s = AddOne(s);
                }
                    step++;
                }
                return step;
            }

            public string AddOne(string s)
            {
                bool flag = false;
                string temp = "";
                int lsp = 1;
                for (int i = s.Length - 1; i >= 0; i--)
                {
                    if (s[i] == '0' && lsp == 1)
                    {
                        return s.Substring(0, i) + "1" + temp;
                    }
                    else
                    {
                        temp = "0" + temp;
                    }
                }
                return "1" + temp;
            }
            private void DataGridviewRefresh()
            {
                ExpenseDataGridViewRefresh();
                CategoryDataGridViewRefresh();
            }

            private void CategoryDataGridViewRefresh()
            {
                categoryDataGridView.DataSource = null;
                categoryDataGridView.DataSource = ExpenseManager.CategoryDictionary.Values.ToList();
                categoryDataGridView.Columns[0].Visible = false;
            }

            private void ExpenseDataGridViewRefresh()
            {

                expenseDataGridView.DataSource = null;
                expenseDataGridView.DataSource = ExpenseManager.ExpenseDictionary.Values.ToList();
                try
                {
                    expenseDataGridView.Columns[0].Visible = false;
                    expenseDataGridView.Columns[1].Visible = false;
                }
                catch
                {

                }
                CategoryenralCBSelectedIndexChanged(this, EventArgs.Empty);
            }

        private bool isMenuPanelVisible = false;
        public void MenuPanelVisible(object sender, EventArgs e)
        {
            if (menuP.Width < 300 && isMenuPanelVisible)
            {
                menuP.Width += 30;
            }
            else if (menuP.Width > 0 && !isMenuPanelVisible)
            {
                menuP.Width -= 30;
            }
            else
            {


                Timer timer = (Timer)(sender);
                timer.Stop();
                timer.Dispose();
            }

        }
        private void MenuViewBtnClick(object sender, EventArgs e)
        {
            //menuP.BringToFront();
            //    isMenuPanelVisible = !isMenuPanelVisible;
            //Timer timer = new Timer();
            //timer.Interval = 2;
            //timer.Tick += MenuPanelVisible;
            //timer.Start();
            menuP.Visible = !menuP.Visible;
        }
        private void ExpensePageClick(object sender, EventArgs e)
        {
            if (mainTabControl.SelectedTab != expenseAddPage)
            {
                switchBtnSelected.BackColor = Color.Transparent;
                mainTabControl.SelectedTab = expenseAddPage;
                expensePageBtn.BackColor = Color.FromArgb(230, 230, 230);
                switchBtnSelected = expensePageBtn;
                expenseAddPage.Controls.Add(expenseDataGridViewP);
                expenseDataGridViewP.BringToFront();
                categoryChangeP.Visible = true;
                commonOperationP.Visible = true;
            }
        }
        private void DayViewBtnClick(object sender, EventArgs e)
        {
            if (mainTabControl.SelectedTab != dayViewPage)
            {
                switchBtnSelected.BackColor = Color.Transparent;
                mainTabControl.SelectedTab = dayViewPage;
                dayViewPageBtn.BackColor = Color.FromArgb(230, 230, 230);
                switchBtnSelected = dayViewPageBtn;
                dayViewPage.Controls.Add(expenseDataGridViewP);
                expenseDataGridViewP.BringToFront();
                categoryChangeP.Visible = true;
                commonOperationP.Visible = true;
            }

        }
        private void CategoryPageBtnClick(object sender, EventArgs e)
        {
            if (mainTabControl.SelectedTab != categoryPage)
            {
                switchBtnSelected.BackColor = Color.Transparent;
                mainTabControl.SelectedTab = categoryPage;
                settingPageBtn.BackColor = Color.FromArgb(230, 230, 230);
                switchBtnSelected = settingPageBtn;

                categoryChangeP.Visible = false;
                commonOperationP.Visible = false;
            }
        }
        private void CustomDateViewPageBtnPClick(object sender, EventArgs e)
        {
            if (mainTabControl.SelectedTab != customDayViewPage)
            {
                switchBtnSelected.BackColor = Color.Transparent;
                mainTabControl.SelectedTab = customDayViewPage;
                expenseSwitchP.BackColor = Color.FromArgb(230, 230, 230);
                switchBtnSelected = expenseSwitchP;
                customDayViewPage.Controls.Add(expenseDataGridViewP);
                expenseDataGridViewP.BringToFront();
                categoryChangeP.Visible = true;
                commonOperationP.Visible = true;
            }
        }
        private void DashBoardBtnClick(object sender, EventArgs e)
        {
            if (mainTabControl.SelectedTab != dashBoardPage)
            {
                switchBtnSelected.BackColor = Color.Transparent;
                mainTabControl.SelectedTab = dashBoardPage;
                dashBoardBtn.BackColor = Color.FromArgb(230, 230, 230);
                switchBtnSelected = dashBoardBtn;
                categoryChangeP.Visible = false;
                commonOperationP.Visible = false;
                DashBoardRefresh();
            }
        }
        private void DashBoardRefresh()
        {
            //  Series series = new Series();
            //   doughnutChart.Series.Clear();
            //  series.ChartType = SeriesChartType.Pie;
            // series.Name = "Category";
            // doughnutChart.Legends.Clear();
            //doughnutChart.Legends.Add(new Legend("Legend"));
            //doughnutChart.Legends["Legend"].Font = new System.Drawing.Font("Arial", 11f);
            pieChart.MaximumSize = new Size(pieChart.Width + 700, pieChart.Height + 400);
            columsChart.Series[0].Points.Clear();
            pieChart.Series[0].Points.Clear();
            dougenutChart.Series[0].Points.Clear();
            //   series["PieLabelStyle"] = "Inside";
            //  series["PieLineColor"] = "Black";
            //   series["PieDrawingStyle"] = "SoftEdge";
            List<Category> categoryList = ExpenseManager.CategoryDictionary.Values.ToList();
            for (int i = 1; i < categoryList.Count; i++)
            {
                pieChart.Series[0].Points.AddXY(categoryList[i].CategoryName, categoryList[i].CurrentMonthUsedAmount);
                columsChart.Series[0].Points.AddXY(categoryList[i].CategoryName, categoryList[i].CurrentMonthUsedAmount);
                dougenutChart.Series[0].Points.AddXY(categoryList[i].CategoryName, categoryList[i].CurrentMonthUsedAmount);
            }
            ChartArea chart = new ChartArea();
            chart.AxisX.Minimum = 1;
            chart.AxisX.Maximum = 12;
            chart.AxisX.Interval = 0.5;
            chart.AxisX.IntervalType = DateTimeIntervalType.Number;
            chart.AxisX.LabelStyle.Format = "";
            lineChart.Series[0].Points.Clear();
            //lineChart.ChartAreas.Add(chart);
            //lineChart.Series[0].IsVisibleInLegend = false;
            //lineChart.Series[0].ChartType = SeriesChartType.Spline;

            for (int month = 1; month <= 12; month++)
            {
                int total = 0;
                for (int day = 1; day <= 31; day++)
                {
                    for (int i = 0; i < expenseDataGridView.Rows.Count; i++)
                    {
                        if (((DateTime)expenseDataGridView.Rows[i].Cells["DateAndTime"].Value).Day == day && month == ((DateTime)expenseDataGridView.Rows[i].Cells["DateAndTime"].Value).Month && ((DateTime)expenseDataGridView.Rows[i].Cells["DateAndTime"].Value).Year == DateTime.Now.Year)
                        {
                            total += int.Parse(expenseDataGridView.Rows[i].Cells["Amount"].Value.ToString());
                        }
                    }
                }
                try
                {
                    lineChart.Series[0].Points.AddXY(month, total);
                }
                catch { }
            }
            string[] months = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

            pieChart.Series[0].IsValueShownAsLabel = false;


        }
     
    }
}
