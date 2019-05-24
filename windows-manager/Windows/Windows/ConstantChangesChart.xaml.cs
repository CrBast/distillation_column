using System.ComponentModel;
using LiveCharts;
using LiveCharts.Configurations;

namespace Arduino_Viewer
{
    /// <summary>
    /// Interaction logic for ConstantChangesChart.xaml
    /// </summary>
    public partial class ConstantChangesChart : INotifyPropertyChanged
    {
        private double _axisMax;
        private double _axisMin;
        private int _counter = 1;

        public ConstantChangesChart()
        {
            InitializeComponent();

            //To handle live data easily, in this case we built a specialized type
            //the MeasureModel class, it only contains 2 properties
            //DateTime and Value
            //We need to configure LiveCharts to handle MeasureModel class
            //The next code configures MeasureModel  globally, this means
            //that LiveCharts learns to plot MeasureModel and will use this config every time
            //a IChartValues instance uses this type.
            //this code ideally should only run once
            //you can configure series in many ways, learn more at 
            //http://lvcharts.net/App/examples/v1/wpf/Types%20and%20Configuration

            var mapper = Mappers.Xy<MeasureModel>()
                .X(model => model.Counter) //use DateTime.Ticks as X
                .Y(model => model.Value); //use the value property as Y

            //lets save the mapper globally.
            Charting.For<MeasureModel>(mapper);

            //the values property will store our values array
            ChartValues = new ChartValues<MeasureModel>();

            AxisMax = 150;

            //The next code simulates data changes every 300 ms

            IsReading = false;

            DataContext = this;
        }

        public ChartValues<MeasureModel> ChartValues { get; set; }
        public double AxisStep { get; set; }
        public double AxisUnit { get; set; }

        public double AxisMax
        {
            get { return _axisMax; }
            set
            {
                _axisMax = value;
                OnPropertyChanged("AxisMax");
            }
        }

        public double AxisMin
        {
            get { return _axisMin; }
            set
            {
                _axisMin = value;
                OnPropertyChanged("AxisMin");
            }
        }

        public bool IsReading { get; set; }

        /// <summary>
        /// Add new Point on Chart
        /// </summary>
        /// <param name="value"></param>
        public void AddPoint(double value)
        {
            ChartValues.Add(new MeasureModel
            {
                Counter = _counter,
                Value = value
            });

            AxisMax = _counter < 150 ? 150 : _counter;
            AxisMin = _counter <= 150 ? 0 : _counter - 150;

            //lets only use the last 150 values
            if (ChartValues.Count > 150) ChartValues.RemoveAt(0);

            if (_counter == 0 || value > 30 || value < 22 )
            {
                ChartValues.RemoveAt(0);
            }

            _counter++;
        }

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}