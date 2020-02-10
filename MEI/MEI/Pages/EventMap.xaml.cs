using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace MEI.Pages
{

    public partial class EventMap : StackLayout
    {

        Map customMap = new Map();
        Geocoder geoCoder = new Geocoder();
        public EventMap()
        {
            InitializeComponent();
            this.Children.Add(customMap);
            GenerateMap();
        }

        public async void GenerateMap()
        {
            try
            {
                ServerVenueMap vm = (await ((HomeLayout)App.Current.MainPage).GetCurrentDomainEventFromServer()).s_event.eventVenueMap;
                customMap.Pins.Clear();
                for (int i = 0; i < vm.venuePoints.Count; i++)
                {
                    Position position = new Position(double.Parse(vm.venuePoints[i].lat), double.Parse(vm.venuePoints[i].lng));
                    var possibleAddresses = await geoCoder.GetAddressesForPositionAsync(position);
                    string pinAddress = "";
                    foreach (var address in possibleAddresses)
                        pinAddress += address + "\n";
                    var pin = new Pin
                    {
                        Type = PinType.Place,
                        Position = new Position(double.Parse(vm.venuePoints[i].lat), double.Parse(vm.venuePoints[i].lng)),
                        Label = vm.venueMapName,
                        Address = pinAddress
                    };
                    if (Device.RuntimePlatform == Device.iOS)
                        pin.MarkerClicked += (s, e) =>
                        {
                            MapEventArgs mapArgs = new MapEventArgs();
                            mapArgs.position = pin.Position;
                            mapArgs.address = pinAddress;
                            App.iosMapClick(this, mapArgs);
                        };
                    customMap.Pins.Add(pin);
                }

                if (customMap.Pins.Count > 0)
                {
                    customMap.IsVisible = true;
                    emptyList.IsVisible = false;
                    customMap.MoveToRegion(MapSpan.FromCenterAndRadius(customMap.Pins[0].Position, Distance.FromMiles(0.2)));
                }
                else
                {
                    customMap.IsVisible = false;
                    emptyList.IsVisible = true;
                    customMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(28.431676, -81.308061), Distance.FromMiles(0.2)));
                }

                await ((HomeLayout)App.Current.MainPage).SetLoading(false, "Loading event sessions...");



            }
            catch
            {

            }
        }

    }
}
