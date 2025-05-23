﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by Reqnroll (https://www.reqnroll.net/).
//      Reqnroll Version:2.0.0.0
//      Reqnroll Generator Version:2.0.0.0
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace MyBddProject.Tests.Features
{
    using Reqnroll;
    using System;
    using System.Linq;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Reqnroll", "2.0.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("Dynamic Preferences")]
    [NUnit.Framework.FixtureLifeCycleAttribute(NUnit.Framework.LifeCycle.InstancePerTestCase)]
    [NUnit.Framework.CategoryAttribute("kira")]
    public partial class DynamicPreferencesFeature
    {
        
        private global::Reqnroll.ITestRunner testRunner;
        
        private static string[] featureTags = new string[] {
                "kira"};
        
        private static global::Reqnroll.FeatureInfo featureInfo = new global::Reqnroll.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Features", "Dynamic Preferences", null, global::Reqnroll.ProgrammingLanguage.CSharp, featureTags);
        
#line 1 "SSC156_DynamicPreferences.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public static async System.Threading.Tasks.Task FeatureSetupAsync()
        {
        }
        
        [NUnit.Framework.OneTimeTearDownAttribute()]
        public static async System.Threading.Tasks.Task FeatureTearDownAsync()
        {
        }
        
        [NUnit.Framework.SetUpAttribute()]
        public async System.Threading.Tasks.Task TestInitializeAsync()
        {
            testRunner = global::Reqnroll.TestRunnerManager.GetTestRunnerForAssembly(featureHint: featureInfo);
            if (((testRunner.FeatureContext != null) 
                        && (testRunner.FeatureContext.FeatureInfo.Equals(featureInfo) == false)))
            {
                await testRunner.OnFeatureEndAsync();
            }
            if ((testRunner.FeatureContext == null))
            {
                await testRunner.OnFeatureStartAsync(featureInfo);
            }
        }
        
        [NUnit.Framework.TearDownAttribute()]
        public async System.Threading.Tasks.Task TestTearDownAsync()
        {
            await testRunner.OnScenarioEndAsync();
            global::Reqnroll.TestRunnerManager.ReleaseTestRunner(testRunner);
        }
        
        public void ScenarioInitialize(global::Reqnroll.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioInitialize(scenarioInfo);
            testRunner.ScenarioContext.ScenarioContainer.RegisterInstanceAs<NUnit.Framework.TestContext>(NUnit.Framework.TestContext.CurrentContext);
        }
        
        public async System.Threading.Tasks.Task ScenarioStartAsync()
        {
            await testRunner.OnScenarioStartAsync();
        }
        
        public async System.Threading.Tasks.Task ScenarioCleanupAsync()
        {
            await testRunner.CollectScenarioErrorsAsync();
        }
        
        public virtual async System.Threading.Tasks.Task FeatureBackgroundAsync()
        {
#line 4
   #line hidden
#line 5
      await testRunner.GivenAsync("a user with the email \"test@test.com\" exists in the system", ((string)(null)), ((global::Reqnroll.Table)(null)), "Given ");
#line hidden
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("User updates theme to Dark Mode")]
        public async System.Threading.Tasks.Task UserUpdatesThemeToDarkMode()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            global::Reqnroll.ScenarioInfo scenarioInfo = new global::Reqnroll.ScenarioInfo("User updates theme to Dark Mode", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 7
   this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((global::Reqnroll.TagHelper.ContainsIgnoreTag(scenarioInfo.CombinedTags) || global::Reqnroll.TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                await this.ScenarioStartAsync();
#line 4
   await this.FeatureBackgroundAsync();
#line hidden
#line 8
      await testRunner.GivenAsync("the user is logged in and on the Preferences page", ((string)(null)), ((global::Reqnroll.Table)(null)), "Given ");
#line hidden
#line 9
      await testRunner.WhenAsync("the user selects \"Dark\" from the Theme dropdown", ((string)(null)), ((global::Reqnroll.Table)(null)), "When ");
#line hidden
#line 10
      await testRunner.ThenAsync("the page should immediately switch to dark mode", ((string)(null)), ((global::Reqnroll.Table)(null)), "Then ");
#line hidden
#line 11
      await testRunner.AndAsync("the \"team_logo_dark.png\" logo should appear", ((string)(null)), ((global::Reqnroll.Table)(null)), "And ");
#line hidden
            }
            await this.ScenarioCleanupAsync();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("User updates font size to large")]
        public async System.Threading.Tasks.Task UserUpdatesFontSizeToLarge()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            global::Reqnroll.ScenarioInfo scenarioInfo = new global::Reqnroll.ScenarioInfo("User updates font size to large", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 13
   this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((global::Reqnroll.TagHelper.ContainsIgnoreTag(scenarioInfo.CombinedTags) || global::Reqnroll.TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                await this.ScenarioStartAsync();
#line 4
   await this.FeatureBackgroundAsync();
#line hidden
#line 14
      await testRunner.GivenAsync("the user is logged in and on the Preferences page", ((string)(null)), ((global::Reqnroll.Table)(null)), "Given ");
#line hidden
#line 15
      await testRunner.WhenAsync("the user selects \"Large\" from the font size dropdown", ((string)(null)), ((global::Reqnroll.Table)(null)), "When ");
#line hidden
#line 16
      await testRunner.ThenAsync("the font size should immediately switch to large", ((string)(null)), ((global::Reqnroll.Table)(null)), "Then ");
#line hidden
            }
            await this.ScenarioCleanupAsync();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("User updates font type to open dyslexic")]
        public async System.Threading.Tasks.Task UserUpdatesFontTypeToOpenDyslexic()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            global::Reqnroll.ScenarioInfo scenarioInfo = new global::Reqnroll.ScenarioInfo("User updates font type to open dyslexic", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 18
   this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((global::Reqnroll.TagHelper.ContainsIgnoreTag(scenarioInfo.CombinedTags) || global::Reqnroll.TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                await this.ScenarioStartAsync();
#line 4
   await this.FeatureBackgroundAsync();
#line hidden
#line 19
      await testRunner.GivenAsync("the user is logged in and on the Preferences page", ((string)(null)), ((global::Reqnroll.Table)(null)), "Given ");
#line hidden
#line 20
      await testRunner.WhenAsync("the user selects \"Open Dyslexic\" from the font type dropdown", ((string)(null)), ((global::Reqnroll.Table)(null)), "When ");
#line hidden
#line 21
      await testRunner.ThenAsync("the font type should immediately switch to open dyslexic", ((string)(null)), ((global::Reqnroll.Table)(null)), "Then ");
#line hidden
            }
            await this.ScenarioCleanupAsync();
        }
    }
}
#pragma warning restore
#endregion
