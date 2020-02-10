using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MEI.Pages
{
    public partial class FloorMapPage : ContentView
    {
        public int currentMapIndex = 0;
        List<ServerFloorMap> maps = new List<ServerFloorMap>();
        List<DayButtonTemplate> mapButtons = new List<DayButtonTemplate>();
        public FloorMapPage()
        {
            InitializeComponent();
            loading.SetBinding(ActivityIndicator.IsRunningProperty, "IsLoading");
            loading.BindingContext = mapImage;
        }

        public async void CreateMapScreen(List<ServerFloorMap> _mapList)
        {
            currentMapIndex = 0;
            mapButtons.Clear();
            floorList.Children.Clear();
            maps = _mapList;
            mapHeading.Text = "";
            mapImage.Source = "";

            for (int i = 0; i < maps.Count; i++)
            {
                DayButtonTemplate d = new DayButtonTemplate();
                d.SetButtonText("Map " + (i + 1).ToString(), i, SetCurrentMap);
                if (i == 0)
                    d.SetButton();
                floorList.Children.Add(d);
                mapButtons.Add(d);
            }

            if (maps.Count > 0)
            {
                emptyList.IsVisible = false;
                floorMaps.IsVisible = true;
                mapButtons[0].SetButton();
                mapHeading.Text = maps[0].eventFloorMapName;
                mapImage.Source = new UriImageSource { Uri = new Uri(maps[0].eventFloorMapURL), CachingEnabled=false };
            }
            else
            {
                floorMaps.IsVisible = false;
                emptyList.IsVisible = true;
            }
            await ((HomeLayout)App.Current.MainPage).SetLoading(false, "Loading event sessions...");
        }

        public void ResetMap()
        {
            zoomContainer.ResetScale();
        }

        public void SetCurrentMap(object s, EventArgs e)
        {            
            if (currentMapIndex != ((DayButtonTemplate)s).buttonID)
            {
                currentMapIndex = ((DayButtonTemplate)s).buttonID;
                for (int i = 0; i < mapButtons.Count; i++)
                {
                    if (((DayButtonTemplate)s).buttonID == i)
                    {
                        mapButtons[i].SetButton();
                    }
                    else
                    {
                        mapButtons[i].ResetButton();
                    }
                }
                mapHeading.Text = maps[currentMapIndex].eventFloorMapName;
                mapImage.Source = maps[currentMapIndex].eventFloorMapURL;
                zoomContainer.ResetScale();
            }

        }
    }
}
