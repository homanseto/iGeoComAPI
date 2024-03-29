﻿using PuppeteerSharp;


namespace iGeoComAPI.Utilities
{
    public class PuppeteerConnection
    {
        public async Task<T> PuppeteerGrabber<T>(string? url, string? infoCode, string? waitSelector)
        {
            BrowserFetcher browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync(BrowserFetcher.DefaultChromiumRevision);

            using (var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = false,
                IgnoreHTTPSErrors = true,
                /*
                Args = new[]
                {
                    "--proxy-server=http://ehmseto_01:YouMeK100@smoproxy:8080/",
                    "--no-sandbox",
                    "--disable-infobars",
                    "--disable-setuid-sandbox",
                    "--ignore-certificate-errors",
                }
                */
            }))
            using (var page = await browser.NewPageAsync())
            {
                //await page.SetRequestInterceptionAsync(true);
                await page.GoToAsync(url);
                await page.WaitForSelectorAsync(waitSelector);
                return await page.EvaluateFunctionAsync<T>(infoCode);
            }
        }

        public async Task<string> GetUrl(string url)
        {
            var timeout = (int)TimeSpan.FromSeconds(5).TotalMilliseconds;
            var options = new NavigationOptions { Timeout = timeout };
            BrowserFetcher browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
            using (var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = false,
                IgnoreHTTPSErrors = true
            })) using (var page = await browser.NewPageAsync())
            {
                new NavigationOptions().WaitUntil = new[]
               {
                    WaitUntilNavigation.Load,
                    WaitUntilNavigation.DOMContentLoaded,
                    WaitUntilNavigation.Networkidle0,
                    WaitUntilNavigation.Networkidle2
                };
                await page.GoToAsync(url);
                await page.WaitForTimeoutAsync(timeout);
                var link = page.Url;
                return link;
            }
        }
    }
}
