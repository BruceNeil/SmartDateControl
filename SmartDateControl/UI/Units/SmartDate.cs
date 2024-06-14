using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SmartDateControl.UI.Units
{
    public class SmartDate : Control
    {
        private CalendarSwitch _switch;
        private CalendarBox _Listbox;
        private Popup _popup;
        private ChevronButton _left;
        private ChevronButton _right;

        public DateTime CurrentMonth
        {
            get { return (DateTime)GetValue(CurrentMonthProperty); }
            set { SetValue(CurrentMonthProperty, value); }
        }

        public static readonly DependencyProperty CurrentMonthProperty =
            DependencyProperty.Register("CurrentMonth", typeof(DateTime), typeof(SmartDate), new PropertyMetadata(null));




        public DateTime?  SelectDate
        {
            get { return (DateTime?)GetValue(SelectDateProperty); }
            set { SetValue(SelectDateProperty, value); }
        }

        public static readonly DependencyProperty SelectDateProperty =
            DependencyProperty.Register("SelectDate", typeof(DateTime?), typeof(SmartDate), new PropertyMetadata(null));



        static SmartDate()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SmartDate), new FrameworkPropertyMetadata(typeof(SmartDate)));
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _switch = (CalendarSwitch)GetTemplateChild("PART_Switch");
            _Listbox = (CalendarBox)GetTemplateChild("PART_ListBox"); 
            _popup = (Popup)GetTemplateChild("PART_Popup");
            _popup.Closed += _popup_Closed;

            _left = (ChevronButton)GetTemplateChild("PART_Left");
            _right = (ChevronButton)GetTemplateChild("PART_Right");

            _switch.Click += _switch_Click;
            _Listbox.MouseLeftButtonUp += _Listbox_PreviewMouseLeftButtonUp;

            _right.Click += (s, e) => MoveMonthClick(1);
            _left.Click += (s, e) => MoveMonthClick(-1);
        }

        private void MoveMonthClick(int v)
        {
            GenerateCalendar(CurrentMonth.AddMonths(v));
        }

        private void _popup_Closed(object? sender, EventArgs e)
        {
            _switch.IsChecked = IsMouseOver;
        }

        private void _Listbox_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_Listbox.SelectedItem is CalendarBoxItem selected)
            {
                SelectDate = selected.Date;
                GenerateCalendar(selected.Date);
            }
        }

        private void _switch_Click(object sender, RoutedEventArgs e)
        {
            if(_switch.IsChecked == true)
            {
                _popup.IsOpen = true;
                GenerateCalendar(DateTime.Now);
            }
        }

        private void GenerateCalendar(DateTime current)
        {
            if(current.ToString("yyyyMM") == CurrentMonth.ToString("yyyyMM")) return;

            CurrentMonth = current;
            _Listbox.Items.Clear();
            DateTime fDayofMonth = new DateTime(current.Year, current.Month, 1);
            DateTime lDayofMonth = fDayofMonth.AddMonths(1).AddDays(-1);
            int foffset = (int)fDayofMonth.DayOfWeek;
            int loffset = 7-(int)lDayofMonth.DayOfWeek;

            DateTime fDay = fDayofMonth.AddDays(-foffset+1);
            DateTime lDay = lDayofMonth.AddDays(loffset);

            for (DateTime day = fDay; day<= lDay; day=day.AddDays(1))
            {
                CalendarBoxItem boxItem = new CalendarBoxItem();
                boxItem.IsCurrentMonth = day.Month == current.Month;
                boxItem.Date = day;
                boxItem.IsToday = (day.Month == DateTime.Now.Month&& day.Day == DateTime.Now.Day);
                boxItem.Content = day.Day;
                _Listbox.Items.Add(boxItem);
            }
        }
    }
}
