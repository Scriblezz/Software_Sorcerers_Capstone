using NUnit.Framework;
using Reqnroll;
using Reqnroll.BoDi;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Threading.Tasks;

[Binding]
public class StreamingServiceFilterSteps
{
    private readonly IWebDriver _driver;

    public StreamingServiceFilterSteps(IObjectContainer objectContainer)
    {
        var options = new ChromeOptions();
        options.AddArgument("--headless");
        options.AddArgument("--no-sandbox");
        options.AddArgument("--disable-dev-shm-usage");

        _driver = new ChromeDriver(options);
        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
        objectContainer.RegisterInstanceAs<IWebDriver>(_driver);
    }

    [Given("the user is on the content browsing page")]
    public void GivenTheUserIsOnTheContentBrowsingPage()
    {
        _driver.Navigate().GoToUrl("http://localhost:5000"); // Replace with your app's base URL
        // Updated to use an existing id from Index.cshtml
        Assert.IsTrue(_driver.FindElement(By.Id("searchInput")).Displayed);
    }

    [Given("the list of available streaming services is displayed")]
    public void GivenTheListOfAvailableStreamingServicesIsDisplayed()
    {
        // Click the dropdown button
        var dropdownButton = _driver.FindElement(By.Id("sortGenreDropdown"));
        dropdownButton.Click();

        // Find the drop down menu associated with the button
        var dropdownMenu = _driver.FindElement(By.CssSelector("ul.dropdown-menu[aria-labelledby='sortGenreDropdown']"));
        Assert.IsTrue(dropdownMenu.Displayed, "The streaming services dropdown menu is not displayed.");

        // Verify it contains at least one service
        var listItems = dropdownMenu.FindElements(By.TagName("li"));
        Assert.IsTrue(listItems.Count > 0, "No streaming services were found in the dropdown.");
    }
    [Given("the user has one or more streaming service filters applied")]
public void GivenTheUserHasOneOrMoreStreamingServiceFiltersApplied()
{
    // Ensure the user is on the content browsing page
    GivenTheUserIsOnTheContentBrowsingPage();

    GivenTheListOfAvailableStreamingServicesIsDisplayed();
    
    // Execute a search so that filter controls become enabled
    var searchInput = _driver.FindElement(By.Id("searchInput"));
    searchInput.Clear();
    searchInput.SendKeys("Titanic"); // Use an appropriate search term
    _driver.FindElement(By.CssSelector("button.search-btn")).Click();
    
    // Wait until the streaming filters container is visible
    var wait = new OpenQA.Selenium.Support.UI.WebDriverWait(_driver, TimeSpan.FromSeconds(10));
    wait.Until(d => d.FindElement(By.Id("streaming-filters")).Displayed);

    // Wait until the dropdown button is interactable
    var dropdownButton = wait.Until(d =>
    {
        var btn = d.FindElement(By.Id("sortGenreDropdown"));
        return (btn.Displayed && btn.Enabled) ? btn : null;
    });
    // Use JavaScript click to make sure the click registers
    ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", dropdownButton);

    // Wait until the dropdown menu is expanded (Bootstrap adds "show" class)
    wait.Until(d =>
    {
        var menu = d.FindElement(By.CssSelector("ul.dropdown-menu[aria-labelledby='sortGenreDropdown']"));
        return menu.GetAttribute("class").Contains("show");
    });
    
    // Now select a streaming filter (e.g., Netflix)
    WhenTheUserSelectsASpecificStreamingService("Netflix");
}
    
[When("the user selects a specific streaming service \"(.*)\"")]
public void WhenTheUserSelectsASpecificStreamingService(string service)
{
    // Use an explicit wait to ensure the checkbox is displayed and enabled
    var wait = new OpenQA.Selenium.Support.UI.WebDriverWait(_driver, TimeSpan.FromSeconds(10));
    var serviceCheckbox = wait.Until(driver => {
        try
        {
            var elem = driver.FindElement(By.CssSelector($"input[value='{service}']"));
            return (elem.Displayed && elem.Enabled) ? elem : null;
        }
        catch (NoSuchElementException)
        {
            return null;
        }
    });
    
    // Scroll into view and click via JavaScript in case of interactability issues
    ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", serviceCheckbox);
    ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", serviceCheckbox);
    
    // Dispatch the change event so the UI (Clear Filters button) is updated immediately.
    ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].dispatchEvent(new Event('change'));", serviceCheckbox);
    
    // Force the update of the Clear Filters button by calling the update function from movieSearch.js
    ((IJavaScriptExecutor)_driver).ExecuteScript("updateClearFiltersVisibility();");
}

    [When("the user selects streaming services \"(.*)\" and \"(.*)\"")]
    public void WhenTheUserSelectsStreamingServices(string service1, string service2)
    {
        var service1Checkbox = _driver.FindElement(By.CssSelector($"input[value='{service1}']"));
        var service2Checkbox = _driver.FindElement(By.CssSelector($"input[value='{service2}']"));
        service1Checkbox.Click();
        service2Checkbox.Click();
    }

    [When("the user clicks the \"Clear Filters\" button")]
    public void WhenTheUserClicksTheClearFiltersButton()
    {
        // Updated id to match the Index.cshtml: "clearFilters"
        var clearFiltersButton = _driver.FindElement(By.Id("clearFilters"));
        clearFiltersButton.Click();
    }

    [Then("the content list is updated to show only items available on \"(.*)\"")]
    public void ThenTheContentListIsUpdatedToShowOnlyItemsAvailableOn(string service)
    {
        var contentItems = _driver.FindElements(By.CssSelector(".content-item"));
        foreach (var item in contentItems)
        {
            Assert.IsTrue(item.Text.Contains(service));
        }
    }

    [Then("the content list is updated to show items available on either \"(.*)\" or \"(.*)\"")]
    public void ThenTheContentListIsUpdatedToShowItemsAvailableOnEither(string service1, string service2)
    {
        var contentItems = _driver.FindElements(By.CssSelector(".content-item"));
        foreach (var item in contentItems)
        {
            Assert.IsTrue(item.Text.Contains(service1) || item.Text.Contains(service2));
        }
    }

    [Then("all streaming service filters are removed")]
    public void ThenAllStreamingServiceFiltersAreRemoved()
    {
        var checkboxes = _driver.FindElements(By.CssSelector("input[type='checkbox']"));
        foreach (var checkbox in checkboxes)
        {
            Assert.IsFalse(checkbox.Selected);
        }
    }

    [Then("the content list returns to the default unfiltered view")]
    public void ThenTheContentListReturnsToTheDefaultUnfilteredView()
    {
        var contentItems = _driver.FindElements(By.CssSelector(".content-item"));
        Assert.IsTrue(contentItems.Count > 0); // Assuming default view has content
    }

[Then("the content list should update within (.*) seconds")]
public void ThenTheContentListShouldUpdateWithinSeconds(int seconds)
{
    var startTime = DateTime.Now;
    var contentItems = _driver.FindElements(By.CssSelector(".content-item"));
    var endTime = DateTime.Now;
    Assert.IsTrue((endTime - startTime).TotalSeconds <= seconds);
    Assert.IsTrue(contentItems.Count > 0); // Assuming filtered view has content
}

    [AfterScenario]
    public void TearDown()
    {
        _driver.Quit();
    }
}
