namespace AISalesDashboard
{
    public partial class AISalesDemo : ContentPage
    {
        private const int SplashScreenDelay = 1000;

        public AISalesDemo()
        {
            InitializeComponent();
            SetInitialContent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            StartSplashScreen();
        }

        private void SetInitialContent()
        {
            this.Content = new SplashScreenView();
        }

        private void StartSplashScreen()
        {
            Dispatcher.Dispatch(async () =>
            {
                await Task.Delay(SplashScreenDelay);
                ShowSampleView();
            });
        }

        private void ShowSampleView()
        {
#if ANDROID || IOS
            SetPlatformContent(new AndroidUI());
#elif WINDOWS || MACCATALYST
            SetPlatformContent(new DesktopUI());
#endif
        }

        private void SetPlatformContent(View platformView)
        {
            this.Content = platformView;
        }
    }
}
