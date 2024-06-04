
namespace Blazor.Components.DataVisualization
{
    partial class USAHeatMap
    {
        /// <summary>
        /// State abbreviation and count for that state.
        /// </summary>
        [Parameter]
        public Dictionary<string, int> MapData { get; set; } = new();

        [Parameter]
        public int PrimaryRGB_Red { get; set; } = 0;

        [Parameter]
        public int PrimaryRGB_Green { get; set; } = 0;

        [Parameter]
        public int PrimaryRGB_Blue { get; set; } = 0;

        private string ViewBox = "0 0 1000 589";

        // Logical Map Data used to generate map with opacity and details
        private List<StateData> States { get; set; } = new();

        // SVG Map Data
        private StateMapListModel StateMap { get; set; } = new();

        private string MapClass { get; set; } = "map-container";
        private MapLabelStyle MapDetailsBoxStyle { get; set; } = new();
        private string MapDetailsStateContent { get; set; } = string.Empty;

        private string MapDetailStyle { get; set; } = string.Empty;

        private class MapLabelStyle
        {
            public int Opacity { get; set; } = 0;
            public double Left { get; set; } = 0;
            public double Top { get; set; } = 0;
        }

        private class StateData
        {
            public string Abbreviation { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public int Count { get; set; }
            public string Location { get; set; } = string.Empty;
            public string Style { get; set; } = "fill: rgba(0,0,0,0)";
        }

        protected override void OnInitialized()
        {
            States = GetStateData();
        }

        private List<StateData> GetStateData()
        {
            var stateData = new List<StateData>();
            var maxCount = MapData.Values.Max();
            int[] data = MapData.Values.ToArray();

            foreach (var state in StateMap.StateMap)
            {
                int count = 0;

                KeyValuePair<string, int>? item = MapData.FirstOrDefault(x => x.Key == state.Abbreviation);
                if (item != null)
                {
                    count = item.Value.Value;
                }

                stateData.Add(new StateData
                {
                    Abbreviation = state.Abbreviation,
                    Name = state.Name,
                    Count = count,
                    Location = state.SvgPath,
                    Style = GetStateFillColorRGB(count, maxCount, data)
                }); 
            }

            return stateData;
        }

        private string GetStateFillColorRGB(int count, int max, int[] data)
        {
            var primaryR = PrimaryRGB_Red;
            var primaryG = PrimaryRGB_Green;
            var primaryB = PrimaryRGB_Blue;

            // Calculate the intensity adjustment based on the value and maxValue
            double intensityRatio = 1.0 - ((double)count / max);

            // Calculate the adjusted RGB values
            int r = (int)(primaryR + (255 - primaryR) * intensityRatio);
            int g = (int)(primaryG + (255 - primaryG) * intensityRatio);
            int b = (int)(primaryB + (255 - primaryB) * intensityRatio);

            return $"stroke-width:0.97063118000000004;fill:rgba({r}, {g}, {b}, 1)";
        }

        private void OnStateMouseEnter(string abbreviation)
        {
            if (string.IsNullOrEmpty(abbreviation))
            {
                MapDetailsBoxStyle.Opacity = 0;
                SetMapDetailsContent(abbreviation);
            }
            else
            {
                MapDetailsBoxStyle.Opacity = 85;
                SetMapDetailsContent(abbreviation);
            }

            UpdateDetailsBoxStyle();
        }

        private void OnStateMouseLeave()
        {
            MapDetailsBoxStyle.Opacity = 0;
            SetMapDetailsContent("");
        }

        private void OnMapMouseMove(MouseEventArgs args)
        {
            MapDetailsBoxStyle.Top = args.ClientY + 30;
            MapDetailsBoxStyle.Left = args.ClientX;
            UpdateDetailsBoxStyle();
        }

        private void SetMapDetailsContent(string abbreviation)
        {
            MapDetailsStateContent = string.Empty;

            if (!string.IsNullOrEmpty(abbreviation))
            {
                var stateData = States.FirstOrDefault(x => x.Abbreviation == abbreviation);

                if (stateData != null)
                {
                    MapDetailsStateContent = $"{stateData.Name} - {stateData.Count}";
                }
            }
        }

        private void UpdateDetailsBoxStyle()
        {
            MapDetailStyle = $"top: {MapDetailsBoxStyle.Top}px; left: {MapDetailsBoxStyle.Left}px; opacity: {MapDetailsBoxStyle.Opacity}%;";
        }
    }
}
