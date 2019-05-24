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

            var mapper = Mappers.Xy<MeasureModel>()
                .X(model => model.Counter) //use DateTime.Ticks as X
                .Y(model => model.Value); //use the value property as Y

            //lets save the mapper globally.
            Charting.For<MeasureModel>(mapper);

            //the values property will store our values array
            ChartValues = new ChartValues<MeasureModel>();

            AxisMax = 150;

            IsReading = false;

            DataContext = this;
        }

        public ChartValues<MeasureModel> ChartValues { get; set; }
        public double AxisStep { get; set; }
        public double AxisUnit { get; set; }

        #region Manage Min/Max Axe X
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
        #endregion

        public bool IsReading { get; set; }

        #region Add new Point
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
        #endregion

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