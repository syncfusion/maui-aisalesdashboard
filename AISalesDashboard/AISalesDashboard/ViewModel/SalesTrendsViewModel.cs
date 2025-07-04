using Syncfusion.Maui.AIAssistView;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace AISalesDashboard
{
    public class SalesTrendsViewModel : BaseViewModel
    {
        #region Properties

        private readonly SalesDataService? _salesDataService;

        private ObservableCollection<Product>? _products = new();
        public ObservableCollection<Product>? Products
        {
            get => _products;
            set
            {
                _products = value;
                OnPropertyChanged(nameof(Products));
            }
        }

        private ObservableCollection<Region>? _regions = new();
        public ObservableCollection<Region>? Regions
        {
            get => _regions;
            set
            {
                _regions = value;
                OnPropertyChanged(nameof(Regions));
            }
        }
   
        private ObservableCollection<SalesData>? _salesData = new();
        public ObservableCollection<SalesData>? SalesData
        {
            get => _salesData;
            set
            {
                _salesData = value;
                OnPropertyChanged(nameof(SalesData));
            }
        }

        private ObservableCollection<SalesData>? _filteredSalesData = new();
        public ObservableCollection<SalesData>? FilteredSalesData
        {
            get => _filteredSalesData;
            set
            {
                _filteredSalesData = value;
                OnPropertyChanged(nameof(FilteredSalesData));
            }
        }

        private List<DateRangeOption>? dateRanges;
        public List<DateRangeOption>? DateRanges
        {
            get => dateRanges;
            set
            {
                dateRanges = value;
                OnPropertyChanged(nameof(DateRanges));
            }
        }

        private decimal? _totalRevenue;
        public decimal? TotalRevenue
        {
            get => _totalRevenue;
            set
            {
                _totalRevenue = value;
                OnPropertyChanged(nameof(TotalRevenue));
                UpdateDashboardLabelCards();
            }
        }

        private decimal? _totalProfit;
        public decimal? TotalProfit
        {
            get => _totalProfit;
            set
            {
                _totalProfit = value;
                OnPropertyChanged(nameof(TotalProfit));
                UpdateDashboardLabelCards();
            }
        }

        private double? _profitMargin;
        public double? ProfitMargin
        {
            get => _profitMargin;
            set
            {
                _profitMargin = value;
                OnPropertyChanged(nameof(ProfitMargin));
                UpdateDashboardLabelCards();
            }
        }

        private double? _growthRate;
        public double? GrowthRate
        {
            get => _growthRate;
            set
            {
                _growthRate = value;
                OnPropertyChanged(nameof(GrowthRate));
                UpdateDashboardLabelCards();
            }
        }

        private bool showAssistView;
        public bool ShowAssistView
        {
            get => showAssistView;
            set
            {
                showAssistView = value;
                OnPropertyChanged(nameof(ShowAssistView));
            }
        }

        private ObservableCollection<IAssistItem>? messages = new();
        public ObservableCollection<IAssistItem>? Messages
        {
            get => messages;
            set
            {
                messages = value;
                OnPropertyChanged(nameof(Messages));
            }
        }

        private ObservableCollection<ProductInfo>? productInfos;
        public ObservableCollection<ProductInfo>? ProductInfos
        {
            get
            {
                return productInfos;
            }
            set
            {
                if (productInfos != value)
                {
                    productInfos = value;
                    OnPropertyChanged(nameof(ProductInfos));
                }
            }
        }

        private ObservableCollection<OrderInfo>? orderInfos;
        public ObservableCollection<OrderInfo>? OrderInfos
        {
            get
            {
                return orderInfos;
            }
            set
            {
                if (orderInfos != value)
                {
                    orderInfos = value;
                    OnPropertyChanged(nameof(OrderInfos));
                }
            }
        }

        public ObservableCollection<Brush>? PaletteBrushes1 { get; set; }

        private ObservableCollection<CustomMarker>? mapMarker;
        public ObservableCollection<CustomMarker>? MapMarkerCollection
        {
            get => mapMarker;
            set
            {
                mapMarker = value;
                OnPropertyChanged(nameof(MapMarkerCollection));
            }
        }

        private List<PageInfo>? pages;
        public  List<PageInfo>? Pages
        {
            get => pages;
            set
            {
                if(pages != value)
                {
                    pages = value;
                    OnPropertyChanged(nameof(Pages));
                }
            }
        }

        public event Action<string>? OnPageSelected;

        private PageInfo? _selectedPage;
        public PageInfo? SelectedPage
        {
            get => _selectedPage;
            set
            {
                if (_selectedPage != value)
                {
                    if (_selectedPage != null)
                        _selectedPage.IsSelected = false;

                    _selectedPage = value;

                    if (_selectedPage != null)
                        _selectedPage.IsSelected = true;

                    OnPropertyChanged(nameof(SelectedPage));
                    OnPageSelected?.Invoke(_selectedPage?.Title ?? string.Empty);
                }
            }
        }

        private bool _hasPreviousPage;
        public bool HasPreviousPage
        {
            get => _hasPreviousPage;
            set
            {
                if (_hasPreviousPage != value)
                {
                    _hasPreviousPage = value;
                    OnPropertyChanged(nameof(HasPreviousPage));
                }
            }
        }

        private bool _hasNextPage;
        public bool HasNextPage
        {
            get => _hasNextPage;
            set
            {
                if (_hasNextPage != value)
                {
                    _hasNextPage = value;
                    OnPropertyChanged(nameof(HasNextPage));
                }
            }
        }

        private bool _hasPreviousProductPage;
        public bool HasPreviousProductPage
        {
            get => _hasPreviousProductPage;
            set
            {
                if (_hasPreviousProductPage != value)
                {
                    _hasPreviousProductPage = value;
                    OnPropertyChanged(nameof(HasPreviousProductPage));
                }
            }
        }

        private bool _hasNextProductPage;
        public bool HasNextProductPage
        {
            get => _hasNextProductPage;
            set
            {
                if (_hasNextProductPage != value)
                {
                    _hasNextProductPage = value;
                    OnPropertyChanged(nameof(HasNextProductPage));
                }
            }
        }

        private bool _hasPreviousOrderPage;
        public bool HasPreviousOrderPage
        {
            get => _hasPreviousOrderPage;
            set
            {
                if (_hasPreviousOrderPage != value)
                {
                    _hasPreviousOrderPage = value;
                    OnPropertyChanged(nameof(HasPreviousOrderPage));
                }
            }
        }

        private bool _hasNextOrderPage;
        public bool HasNextOrderPage
        {
            get => _hasNextOrderPage;
            set
            {
                if (_hasNextOrderPage != value)
                {
                    _hasNextOrderPage = value;
                    OnPropertyChanged(nameof(HasNextOrderPage));
                }
            }
        }

        private ObservableCollection<DashboardLabelCardModel>? homepagecardLabels;
        public ObservableCollection<DashboardLabelCardModel>? HomePageCardLabels
        {
            get => homepagecardLabels;
            set
            {
                if(homepagecardLabels != value)
                {
                    homepagecardLabels = value;
                    OnPropertyChanged(nameof(HomePageCardLabels));
                }
            }
        }

        private ObservableCollection<ISuggestion> _suggestions;
        public ObservableCollection<ISuggestion> Suggestions
        {
            get => _suggestions;
            set
            {
                if(_suggestions != value)
                {
                    _suggestions = value;
                    OnPropertyChanged(nameof(Suggestions));
                }
            }
        }

        private const int SalesPageSize =5;

#if MACCATALYST
        private int ProductPageSize = 6;
        private const int OrderPageSize = 16;
#elif IOS
        private int ProductPageSize = 5;
        private const int OrderPageSize = 8;
#else
        private int ProductPageSize = 5;
        private const int OrderPageSize = 9;
#endif
        private int currentproductPage;
        private int currentsalesPage;
        private int currentorderpage;

        public ObservableCollection<ProductInfo> ProductPagedItems { get; } = new();

        public ObservableCollection<SalesData> SalesPagedItems { get; } = new();
        public ObservableCollection<OrderInfo> OrderPagedItems { get; } = new();


        public ICommand NextProductPageCommand { get; }
        public ICommand PreviousProductPageCommand { get; }

        public ICommand NextSalesPageCommand { get; }
        public ICommand PreviousSalesPageCommand { get; }

        public ICommand NextOrderPageCommand { get; }
        public ICommand PreviousOrderPageCommand { get; }

        public string OrderPageInfo => $"Page {currentorderpage + 1} of {TotalOrderPages}";
        private int TotalOrderPages => (int)Math.Ceiling((double)OrderInfos!.Count / OrderPageSize);

        public string ProductPageInfo => $"Page {currentproductPage + 1} of {TotalProductPages}";
        private int TotalProductPages => (int)Math.Ceiling((double)ProductInfos!.Count / ProductPageSize);

        public string SalesPageInfo => $"Page {currentsalesPage + 1} of {TotalSalesPages}";
        private int TotalSalesPages => (int)Math.Ceiling((double)FilteredSalesData!.Count / SalesPageSize);
#endregion

        #region Constructor

        public SalesTrendsViewModel(SalesDataService salesDataService)
        {

            PaletteBrushes1 = new ObservableCollection<Brush>(
        (Application.Current!.UserAppTheme == AppTheme.Light || Application.Current!.UserAppTheme == AppTheme.Unspecified) ?
        new[]
                {
            "#CAC4D0", "#116DF9", "#E2227E", "#25E739", "#F4890B", "#00E190", "#FF4E4E"
        }.Select(c => new SolidColorBrush(Color.FromArgb(c))) :
        new[]
            {
            "#49454F", "#BF3B49", "#33B677", "#C9588E", "#8B40C6", "#588249", "#DA9646"
        }.Select(c => new SolidColorBrush(Color.FromArgb(c)))
            );

            Pages = new List<PageInfo>
            {
               new() { Title = "Home", PageIcon = "\ue738" },
               new() { Title = "Products", PageIcon = "\ue82a" },
               new() { Title = "Orders", PageIcon = "\ue73e" },
               new() { Title = "Predictions", PageIcon = "\ue820" }
            };

            _suggestions = new ObservableCollection<ISuggestion>
            {
                new AssistSuggestion() {Text = "Top performing products?"},
                new AssistSuggestion() {Text = "Sales trend last month?"},
                new AssistSuggestion() {Text = "Region with highest growth?"}
            };

            SelectedPage = Pages.FirstOrDefault(p => p.Title == "Home");

            ProductInfos = new ObservableCollection<ProductInfo>(LoadProductData());

            OrderInfos = Orders(1);
            MapMarkerCollection = new ObservableCollection<CustomMarker>();
            _salesDataService = salesDataService;
            _ = Initialize();
            _ = LoadDashboardData();

            DateRanges = new List<DateRangeOption>
            {
              new() { DisplayText = "Last 30 Days", Value = DateRange.Last30Days },
              new() { DisplayText = "Last Quarter", Value = DateRange.LastQuarter },
              new() { DisplayText = "Year to Date", Value = DateRange.YearToDate },
              new() { DisplayText = "Last Year", Value = DateRange.LastYear }
            };

            HomePageCardLabels = new ObservableCollection<DashboardLabelCardModel>();
            UpdateDashboardLabelCards();

            NextOrderPageCommand = new Command(NextOrderPage);
            PreviousOrderPageCommand = new Command(PreviousOrderPage);
            NextProductPageCommand = new Command(NextProductPage);
            PreviousProductPageCommand = new Command(PreviousProductPage);
            NextSalesPageCommand = new Command(NextSalesPage);
            PreviousSalesPageCommand = new Command(PreviousSalesPage);

            LoadProductsPage();
            LoadOrderPage();
        }

        #endregion

        #region Methods

        private ObservableCollection<OrderInfo> Orders(int month)
        {
            var random = new Random();
            var customers = new[] { "Alen", "James", "Reena", "Mark", "Teena", "Thomas", "John", "Lawrence", "Samwell", "Arya", "Jennifer", "Robert", "Lilly", "Jessica", "Grace", "Elizabeth", "Melana", "Arthur", "Michael", "George" };
            var yesterday = DateTime.Now.Date.AddDays(-1);
            var statuses = Enum.GetValues(typeof(Status)).Cast<Status>().ToArray();
            var idRange = Enumerable.Range(100001, 16).ToList();
            var count = 0;

#if WINDOWS || ANDROID 
            count = 9;
#elif MACCATALYST
            count = 16;
#elif IOS
        count = 8;
#endif

            var monthlyOrders = new ObservableCollection<OrderInfo>(
                Enumerable.Range(0, count).Select(_ =>
                {
                var product = ProductInfos![random.Next(ProductInfos.Count)];
                    var marginValue = random.NextDouble() * 20;
                    var status = statuses[random.Next(statuses.Length)];
                    var randomDateTime = yesterday.AddMinutes(random.Next(0, 48 * 60));
                    return new OrderInfo
                {
                        Id = idRange[random.Next(idRange.Count)],
                    Product = product.Product,
                    Image = product.Image,
                    OrderStatus = status.ToString(),
                    Brush = GetColorForStatus(status),
                    Customer = customers[random.Next(customers.Length)],
                    OrderDate = randomDateTime.ToString("MMM d, yyyy\nh:mm tt"),
                    ActualPrice = product.BuyingPrice,
                    MarginValue = marginValue,
                    SellingPrice = product.BuyingPrice + marginValue
                    };
                }).ToList());
            return monthlyOrders;
        }

        private void UpdateDashboardLabelCards()
        {
            HomePageCardLabels!.Clear();

            HomePageCardLabels.Add(new DashboardLabelCardModel
            {
                Title = "Total Revenue",
                Value = TotalRevenue.HasValue ? $"${TotalRevenue.Value:N0}" : "$0",
                Icon = "\ue828", IconColor = Color.FromArgb("#116DF9"), Background = Color.FromArgb("#CFE2FF"),
                ValueColor = (Color)(Application.Current!.RequestedTheme == AppTheme.Dark
                            ? Application.Current.Resources["SalesDemoContentLabelTextColorDark"]
                            : Application.Current.Resources["SalesDemoContentLabelTextColor"])
            });

            HomePageCardLabels.Add(new DashboardLabelCardModel
            {
                Title = "Total Profit",
                Value = TotalProfit.HasValue ? $"${TotalProfit.Value:N0}" : "$0",
                Icon = "\ue82c",
                IconColor = Color.FromArgb("#E2227E"),
                Background = Color.FromArgb("#FFDAEC"),
                ValueColor = (Color)(Application.Current.RequestedTheme == AppTheme.Dark
                            ? Application.Current.Resources["SalesDemoContentLabelTextColorDark"]
                            : Application.Current.Resources["SalesDemoContentLabelTextColor"])
            });

            HomePageCardLabels.Add(new DashboardLabelCardModel
            {
                Title = "Profit Margin",
                Value = ProfitMargin.HasValue ? $"+{ProfitMargin.Value:N1}%" : "0%",
                Icon = "\ue82b",
                IconColor = Color.FromArgb("#F4890B"),
                Background = Color.FromArgb("#FFE4C4"),
                ValueColor = Color.FromArgb("#15A03D")
            });

            HomePageCardLabels.Add(new DashboardLabelCardModel
            {
                Title = "Growth Rate",
                Value = GrowthRate.HasValue ? $"{GrowthRate.Value:N1}%" : "0%",
                Icon = "\ue823",
                IconColor = Color.FromArgb("#1BC92D"),
                Background = Color.FromArgb("#E4FFE7"),
                ValueColor = Color.FromArgb("#DC2626")
            });
        }


        private Color GetColorForStatus(Status status)
        {
            var lightThemeColors = new Dictionary<Status, string>
            {
                { Status.Delivered, "#25E739" },
                { Status.Shipped, "#116DF9" },
                { Status.Cancelled, "#FF4E4E" },
                { Status.Refunded, "#F4890B" }
            };

            var darkThemeColors = new Dictionary<Status, string>
            {
                { Status.Delivered, "#33B677" },
                { Status.Shipped, "#BF3B49" },
                { Status.Cancelled, "#9B4848" },
                { Status.Refunded, "#DA9646" }
            };

            return Color.FromArgb(
                (Application.Current!.UserAppTheme == AppTheme.Light || Application.Current!.UserAppTheme == AppTheme.Unspecified ?
                lightThemeColors : darkThemeColors).GetValueOrDefault(status, Colors.Gray.ToHex()));
        }

        public async Task LoadDashboardData()
        {
            // Load financial summary
            try
            {
                var summary = await _salesDataService!.GetSalesSummaryAsync(SelectedDateRange);
                (TotalRevenue, TotalProfit) = (summary["TotalRevenue"], summary["TotalProfit"]);
                (ProfitMargin, GrowthRate) = ((double)summary["AverageProfitMargin"], (double)summary["RevenueGrowth"]);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading dashboard data: {ex.Message}");
            }
        }

        public async Task Initialize()
        {
            try
            {
                var products = await _salesDataService!.GetProductsAsync();
                Products = new ObservableCollection<Product>(products
                    .Select(x => new Product { Name = x.Name, BasePrice = Math.Round(x.BasePrice, 0) })
                    .OrderByDescending(p => p.BasePrice)
                    .Take(5));
                
                Regions = new ObservableCollection<Region>(await _salesDataService.GetRegionsAsync());
                MapMarkerCollection = new ObservableCollection<CustomMarker>(
                    Regions.Select(region => new CustomMarker
                    {
                        Latitude = region.Latitude,
                        Longitude = region.Longitude,
                        Name = region.Country
                    }));
                await LoadSalesData();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error generating predictions: {ex.Message}");
            }
        }

        private List<ProductInfo> LoadProductData()
        {
            var random = new Random();
            return new List<ProductInfo>()
            {
                new ProductInfo { Product = "Samsung Galaxy S3", BuyingPrice = 149.99, Image = "samsung.png", ProductStockCount = 14, ProductRating = random.Next(3,5), ProductID = random.Next(01,30), TotalSales = random.Next(100, 300) },
                new ProductInfo { Product = "iPhone 4", BuyingPrice = 199.99, Image = "iphone_4k.png", ProductStockCount = 8, ProductRating = random.Next(3,5), ProductID = random.Next(01,30), TotalSales = random.Next(100, 300) },
                new ProductInfo { Product = "Google Nexus 5", BuyingPrice = 129.99, Image = "googlemobile.png", ProductStockCount = 2, ProductRating = random.Next(3,5), ProductID = random.Next(01,30), TotalSales = random.Next(100, 300) },
                new ProductInfo { Product = "HTC One M7", BuyingPrice = 169.99, Image = "htconemobile.png", ProductStockCount = 0, ProductRating = random.Next(3,5), ProductID = random.Next(01,30), TotalSales = random.Next(100, 300) },
                new ProductInfo { Product = "BlackBerry Bold 9900", BuyingPrice = 119.99, Image = "blackberry_bold.png", ProductStockCount = 0, ProductRating = random.Next(3,5), ProductID = random.Next(01,30), TotalSales = random.Next(100, 300) },
                new ProductInfo { Product = "Sony Xperia Z1", BuyingPrice = 179.99, Image = "sonyxperia.png", ProductStockCount = 4, ProductRating = random.Next(3,5), ProductID = random.Next(01,30), TotalSales = random.Next(100, 300) },
                new ProductInfo { Product = "LG G2", BuyingPrice = 149.99, Image = "lgmobile.png", ProductStockCount = 16, ProductRating = random.Next(3,5) , ProductID = random.Next(01, 30), TotalSales = random.Next(100, 300) },
                new ProductInfo { Product = "Nokia Lumia 920", BuyingPrice = 139.99, Image = "nokia_lumia.png", ProductStockCount = 3, ProductRating = random.Next(3,5), ProductID = random.Next(01,30), TotalSales = random.Next(100, 300) },
                new ProductInfo { Product = "Motorola Moto X (2013)", BuyingPrice = 129.99, Image = "motorola_moto.png", ProductStockCount = 8, ProductRating = random.Next(3, 5), ProductID = random.Next(01, 30), TotalSales = random.Next(100, 300)},
                new ProductInfo { Product = "Huawei Ascend P6", BuyingPrice = 159.99, Image = "huawei_ascend.png", ProductStockCount = 25, ProductRating = random.Next(3, 5), ProductID = random.Next(01, 30), TotalSales = random.Next(100, 300)},
                new ProductInfo { Product = "Samsung Galaxy Note 4", BuyingPrice = 179.99, Image = "samsunggalaxy.png", ProductStockCount = 2, ProductRating = random.Next(3, 5), ProductID = random.Next(01, 30), TotalSales = random.Next(100, 300)},
                new ProductInfo { Product = "iPhone 5S", BuyingPrice = 199.99, Image = "iphone_4k.png", ProductStockCount = 28, ProductRating = random.Next(3, 5), ProductID = random.Next(01, 30), TotalSales = random.Next(100, 300)},
                new ProductInfo { Product = "Laptop", BuyingPrice = 899.99, Image = "laptop.png", ProductStockCount = 10, ProductRating = random.Next(3, 5), ProductID = random.Next(01, 30), TotalSales = random.Next(100, 300)},
                new ProductInfo { Product = "Smartphone", BuyingPrice = 699.99, Image = "smartphone.png", ProductStockCount = 25, ProductRating = random.Next(3, 5), ProductID = random.Next(01, 30), TotalSales = random.Next(100, 300)},
                new ProductInfo { Product = "Wireless Earbuds", BuyingPrice = 129.99, Image = "wireless_earbuds.png", ProductStockCount = 15, ProductRating = random.Next(3,5) , ProductID = random.Next(01, 30), TotalSales = random.Next(100, 300)},
                new ProductInfo { Product = "Smartwatch", BuyingPrice = 199.99, Image = "smartwatch.png", ProductStockCount = 20, ProductRating = random.Next(3,5) , ProductID = random.Next(01, 30), TotalSales = random.Next(100, 300)},
                new ProductInfo { Product = "Bluetooth Speaker", BuyingPrice = 79.99, Image = "bluetooth_speaker.png", ProductStockCount = 5, ProductRating = random.Next(3,5) , ProductID = random.Next(01, 30), TotalSales = random.Next(100, 300)},
                new ProductInfo { Product = "Gaming Console", BuyingPrice = 499.99, Image = "gaming_console.png", ProductStockCount = 0, ProductRating = random.Next(3,5), ProductID = random.Next(01, 30), TotalSales = random.Next(100, 300)},
                new ProductInfo { Product = "Tablet", BuyingPrice = 349.99, Image = "tablet.png", ProductStockCount = 12, ProductRating = random.Next(3,5) , ProductID = random.Next(01, 30), TotalSales = random.Next(100, 300)},
                new ProductInfo { Product = "Monitor", BuyingPrice = 229.99, Image = "monitor.png", ProductStockCount = 18, ProductRating = random.Next(3, 5), ProductID = random.Next(01, 30), TotalSales = random.Next(100, 300)},
                new ProductInfo { Product = "Mechanical Keyboard", BuyingPrice = 129.99, Image = "mech_keyboard.png", ProductStockCount = 22, ProductRating = random.Next(3,5) , ProductID = random.Next(01, 30), TotalSales = random.Next(100, 300)},
                new ProductInfo { Product = "Gaming Mouse", BuyingPrice = 59.99, Image = "gaming_mouse.png", ProductStockCount = 30, ProductRating = random.Next(3,5) , ProductID = random.Next(01, 30), TotalSales = random.Next(100, 300)},
                new ProductInfo { Product = "VR Headset", BuyingPrice = 399.99, Image = "vr_headset.png", ProductStockCount = 7, ProductRating = random.Next(3,5) , ProductID = random.Next(01, 30), TotalSales = random.Next(100, 300)},
                new ProductInfo { Product = "External SSD", BuyingPrice = 149.99, Image = "external_ssd.png", ProductStockCount = 16, ProductRating = random.Next(3,5) , ProductID = random.Next(01, 30), TotalSales = random.Next(100, 300)},
                new ProductInfo { Product = "Router", BuyingPrice = 99.99, Image = "routersales.png", ProductStockCount = 0, ProductRating = random.Next(3,5) , ProductID = random.Next(01, 30), TotalSales = random.Next(100, 300)},
                new ProductInfo { Product = "Smart TV", BuyingPrice = 599.99, Image = "smart_tv.png", ProductStockCount = 14, ProductRating = random.Next(3,5) , ProductID = random.Next(01, 30), TotalSales = random.Next(100, 300)},
                new ProductInfo { Product = "Webcam", BuyingPrice = 89.99, Image = "webcam.png", ProductStockCount = 3 , ProductRating = random.Next(3,5) , ProductID = random.Next(01, 30), TotalSales = random.Next(100, 300)},
                new ProductInfo { Product = "Microphone", BuyingPrice = 139.99, Image = "microphone.png", ProductStockCount = 11, ProductRating = random.Next(3,5) , ProductID = random.Next(01, 30), TotalSales = random.Next(100, 300)},
                new ProductInfo { Product = "Graphics Card", BuyingPrice = 699.99, Image = "graphics_card.png", ProductStockCount = 4, ProductRating = random.Next(3,5) , ProductID = random.Next(01, 30), TotalSales = random.Next(100, 300)},
                new ProductInfo { Product = "Power Bank", BuyingPrice = 49.99, Image = "power_bank.png", ProductStockCount = 21 , ProductRating = random.Next(3, 5), ProductID = random.Next(01, 30), TotalSales = random.Next(100, 300)},
            };
        }
        
        private void LoadProductsPage()
        {
            ProductPagedItems.Clear();
            int startIndex = currentproductPage * ProductPageSize;
            ProductInfos?
                .Skip(startIndex)
                .Take(ProductPageSize)
                .ToList()
                .ForEach(ProductPagedItems.Add);

            OnPropertyChanged(nameof(ProductPageInfo));
            UpdateProductPagesNavigationProperties();
        }

        private void UpdateSalesPageNavigationProperties()
        {
            HasPreviousPage = currentsalesPage > 0;
            HasNextPage = currentsalesPage < TotalSalesPages - 1;
        }
        private void UpdateProductPagesNavigationProperties()
        {
            HasPreviousProductPage = currentproductPage > 0;
            HasNextProductPage = currentproductPage < TotalProductPages - 1;
        }

        private void UpdateOrderPagesNavigationProperties()
        {
            HasPreviousOrderPage = currentorderpage > 0;
            HasNextOrderPage = currentorderpage < TotalOrderPages - 1;
        }

        private void LoadSalesPage()
        {
            SalesPagedItems.Clear();
            FilteredSalesData?
                .Skip(currentsalesPage * SalesPageSize)
                .Take(SalesPageSize)
                .ToList()
                .ForEach(SalesPagedItems.Add);

            OnPropertyChanged(nameof(SalesPageInfo));
            UpdateSalesPageNavigationProperties();

        }

        private void NextProductPage()
        {
            if (currentproductPage < TotalProductPages - 1)
            {
                currentproductPage++;
                LoadProductsPage();
            }
        }

        private void PreviousProductPage()
        {
            if (currentproductPage > 0)
            {
                currentproductPage--;
                LoadProductsPage();
            }
        }

        private void NextSalesPage()
        {
            if (currentsalesPage < TotalSalesPages - 1)
            {
                currentsalesPage++;
                LoadSalesPage();
            }
        }

        private void PreviousSalesPage()
        {
            if (currentsalesPage > 0)
            {
                currentsalesPage--;
                LoadSalesPage();
            }
        }
        private void LoadOrderPage()
        {
            OrderPagedItems.Clear();

            int startIndex = currentorderpage * OrderPageSize;
            orderInfos?
                .Skip(startIndex)
                .Take(OrderPageSize)
                .ToList()
                .ForEach(OrderPagedItems.Add);

            OnPropertyChanged(nameof(OrderPageInfo));
            UpdateOrderPagesNavigationProperties();

        }

        private void NextOrderPage()
        {
            if (currentorderpage < TotalOrderPages - 1)
            {
                currentorderpage++;
                LoadOrderPage();
            }
        }

        private void PreviousOrderPage()
        {
            if (currentorderpage > 0)
            {
                currentorderpage--;
                LoadOrderPage();
            }
        }

        public async Task LoadSalesData()
        {
            try
            {
                var data = await LoadSalesDataAsync();
                var random = new Random();

                DateTime startDate = SelectedDateRange.StartDate;
                DateTime endDate = SelectedDateRange.EndDate;
                const int totalDays = 365;
                const int baseRevenue = 500000;
                const int peakRevenue = 1700000;

                double midPoint = totalDays * 0.5;
                double spread = totalDays / 6.0;

                var chartData = Enumerable.Range(0, totalDays).Select(i =>
                {
                    DateTime datePoint = startDate.AddDays(i);
                    double gaussianFactor = Math.Exp(-Math.Pow(i - midPoint, 2) / (2 * Math.Pow(spread, 2)));
                    int revenue = (int)(baseRevenue + gaussianFactor * (peakRevenue - baseRevenue));

                    return new SalesData
                    {
                        Date = datePoint,
                        Revenue = revenue,
                        Quantity = random.Next(200, 1000),
                        Cost = random.Next(200000, 300000),
                        ProductName = data[i].ProductName
                    };
                }).Where(data => data.Date >= startDate && data.Date <= endDate)
                .OrderBy(data => data.Date)
                .ToList();

                SalesData = new ObservableCollection<SalesData>(chartData);
                ApplyFilters(data);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error generating predictions: {ex.Message}");
            }
        }

        private async Task<List<SalesData>> LoadSalesDataAsync() =>
            SelectedDateRange != null
                ? await _salesDataService!.GetSalesDataAsync(SelectedDateRange)
                : new List<SalesData>();

        public void ApplyFilters(List<SalesData> salesData)
        {
            if (salesData == null || salesData.Count == 0)
                return;

            FilteredSalesData = new ObservableCollection<SalesData>(
                salesData.OrderByDescending(x => x.Date).Take(50));

            LoadSalesPage();
        }

#endregion
    }
}
