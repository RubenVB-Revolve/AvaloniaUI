using Avalonia.Controls;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AvaloniaApplication1.Views
{
    public partial class MainWindow : Window
    {
        private double[] _xData;
        private List<double> _latData;
        private List<double> _lonData;
        private List<double> _altData;
        private ClientWebSocket _webSocket;
        private int _dataIndex;

        // Use dynamic types or var for scatter plot references
        private dynamic _scatterPlotLat;
        private dynamic _scatterPlotLon;
        private dynamic _scatterPlotAlt;

        public MainWindow()
        {
            InitializeComponent();

            // Initialize the x-axis data and the data lists for lat, lon, alt
            _xData = new double[1000];
            _latData = new List<double>(new double[1000]);
            _lonData = new List<double>(new double[1000]);
            _altData = new List<double>(new double[1000]);

            // Add scatter plots for lat, lon, and alt and store references
            _scatterPlotLat = PlotView.Plot.Add.Scatter(_xData, _latData.ToArray());
            _scatterPlotLon = PlotView.Plot.Add.Scatter(_xData, _lonData.ToArray());
            _scatterPlotAlt = PlotView.Plot.Add.Scatter(_xData, _altData.ToArray());

            PlotView.Plot.Legend.IsVisible = true;

            // Set the axis limits using the correct method for ScottPlot 5.x
            PlotView.Plot.Axes.SetLimits(0, 1000, -180, 180);

            // Start the WebSocket connection
            StartWebSocket().ConfigureAwait(false);
        }

        private async Task StartWebSocket()
        {
            _webSocket = new ClientWebSocket();
            var uri = new Uri("ws://localhost:8080/ws/datastream");
            await _webSocket.ConnectAsync(uri, CancellationToken.None);

            // Start receiving messages from the WebSocket server
            await ReceiveMessages();
        }

        private async Task ReceiveMessages()
        {
            byte[] buffer = new byte[1024 * 4];
            while (_webSocket.State == WebSocketState.Open)
            {
                WebSocketReceiveResult result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                string message = Encoding.UTF8.GetString(buffer, 0, result.Count);

                // Parse the JSON message
                ParseDataMessage(message);

                // Refresh the plot after updating the data
                PlotView.Refresh();
            }
        }

        private void ParseDataMessage(string jsonMessage)
        {
            var dataMessage = JsonConvert.DeserializeObject<DataMessage>(jsonMessage);

            // Update the x-axis with new timestamp data
            if (_dataIndex < _xData.Length)
            {
                _xData[_dataIndex] = _dataIndex;
            }
            else
            {
                // Scroll the x-axis data if buffer is full
                Array.Copy(_xData, 1, _xData, 0, _xData.Length - 1);
                _xData[^1] = _dataIndex;
            }

            _dataIndex++;

            // Update the latitude, longitude, and altitude values
            if (dataMessage.Data.ContainsKey("vcu/14.InsEstimates1.lat"))
            {
                double lat = Convert.ToDouble(dataMessage.Data["vcu/14.InsEstimates1.lat"]);
                UpdateData(_latData, lat);
            }

            if (dataMessage.Data.ContainsKey("vcu/14.InsEstimates1.lon"))
            {
                double lon = Convert.ToDouble(dataMessage.Data["vcu/14.InsEstimates1.lon"]);
                UpdateData(_lonData, lon);
            }

            if (dataMessage.Data.ContainsKey("vcu/14.InsEstimates1.alt"))
            {
                double alt = Convert.ToDouble(dataMessage.Data["vcu/14.InsEstimates1.alt"]);
                UpdateData(_altData, alt);
            }

            // Update existing scatter plots with new data
            _scatterPlotLat.Update(_xData, _latData.ToArray());
            _scatterPlotLon.Update(_xData, _lonData.ToArray());
            _scatterPlotAlt.Update(_xData, _altData.ToArray());
        }

        private void UpdateData(List<double> dataList, double newValue)
        {
            // Maintain a fixed buffer size of 1000 points by scrolling
            if (dataList.Count >= 1000)
            {
                dataList.RemoveAt(0);
            }
            dataList.Add(newValue);
        }
    }

    // DataMessage class that mirrors the incoming JSON structure
    public class DataMessage
    {
        public long Timestamp { get; set; }
        public Dictionary<string, object> Data { get; set; }
    }
}
