using KidsIdKit.Core.Data;
using Microsoft.AspNetCore.Components;
using System.Reflection;
using System.Threading.Tasks;
using ChildRecord = KidsIdKit.Core.Data.Child;
using SocialMediaAccountsPage = KidsIdKit.Core.Pages.SocialMediaAccounts.SocialMediaAccounts;
using KidsIdKit.Core.Pages;
using KidsIdKit.Core.Pages.CareProviders;
using KidsIdKit.Core.Pages.DistinguishingFeatures;
using KidsIdKit.Core.Pages.FamilyMembers;
using KidsIdKit.Core.Pages.Friends;
using KidsIdKit.Core.Services;
using KidsIdKit.Core.SharedComponents;
using static KidsIdKit.Core.SharedComponents.RemovalConfirmation;
using Moq;

namespace KidsIdKit.Tests.KidsIdKit.Core.Pages;

public class RemovalConfirmationTests : TestContext
{
    private const string ExpectedHeader = "Confirm removal";
    private const string ExpectedMessage = "Are you sure you want to remove\n'{0}'?";

    private readonly Mock<IFamilyStateService> _familyStateService = new();

    public RemovalConfirmationTests()
    {
        Services.AddSingleton<IPageState, PageState>();
        Services.AddSingleton<IFamilyStateService>(_familyStateService.Object);
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public async Task ShowAlert_ForChild_PersistsConfirmationInPageState()
    {
        var child = new ChildRecord { Id = 1 };
        child.ChildDetails.GivenName = "Avery";
        child.ChildDetails.FamilyName = "Jordan";
        var pageState = new PageState();
        pageState.InitStateItem<string>(AlertStateInformationState, string.Empty);
        pageState.InitStateItem<string>(AlertTitleState, string.Empty);
        pageState.InitStateItem<string>(AlertMessageState, string.Empty);
        pageState.InitStateItem<bool>(AlertShowState, false);
        var page = new Kids();
        var pageStateProperty = typeof(Kids).GetProperty("PageState", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        Assert.NotNull(pageStateProperty);
        pageStateProperty.SetValue(page, pageState);

        page.ShowAlert(child);

        Assert.Equal(ExpectedHeader, pageState.GetStateItem<string>(AlertTitleState).Value);
        Assert.Equal(string.Format(ExpectedMessage, "Avery Jordan"), pageState.GetStateItem<string>(AlertMessageState).Value);
        Assert.True(pageState.GetStateItem<bool>(AlertShowState).Value);

        pageState.AppSuspended = true;
        var recreatedPage = new Kids();
        pageStateProperty.SetValue(recreatedPage, pageState);
        var onInitialized = typeof(Kids).GetMethod("OnInitializedAsync", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.NotNull(onInitialized);
        await (Task)onInitialized.Invoke(recreatedPage, null)!;

        Assert.Equal(string.Format(ExpectedMessage, "Avery Jordan"), pageState.GetStateItem<string>(AlertMessageState).Value);
        Assert.True(pageState.GetStateItem<bool>(AlertShowState).Value);
    }

    [Fact]
    public void ShowAlert_ForCareProvider_ShowsCareProviderNameInConfirmation()
    {
        var child = CreateChild();
        _familyStateService.Setup(service => service.GetChild(1)).Returns(child);
        var provider = new CareProvider { Id = 2, GivenName = "Taylor", FamilyName = "Morgan" };
        child.ProfessionalCareProviders.Add(provider);
        var cut = RenderComponent<ChildCareProviders>(parameters => parameters.Add(component => component.id, 1));
        cut.Instance.ShowAlert(provider);
        cut.Render();

        var pageState = Services.GetRequiredService<IPageState>();
        pageState.AppSuspended = true;
        cut.Dispose();
        var recreated = RenderComponent<ChildCareProviders>(parameters => parameters.Add(component => component.id, 1));
        AssertAlert(recreated, "Taylor Morgan");
    }

    [Fact]
    public void ShowAlert_ForDistinguishingFeature_ShowsDescriptionInConfirmation()
    {
        var child = CreateChild();
        _familyStateService.Setup(service => service.GetChild(1)).Returns(child);
        var feature = new DistinguishingFeature { Id = 3, Description = "Scar" };
        child.DistinguishingFeatures.Add(feature);
        var cut = RenderComponent<ChildDistinguishingFeatures>(parameters => parameters.Add(component => component.Id, 1));
        cut.Instance.ShowAlert(feature);
        cut.Render();

        var pageState = Services.GetRequiredService<IPageState>();
        pageState.AppSuspended = true;
        cut.Dispose();
        var recreated = RenderComponent<ChildDistinguishingFeatures>(parameters => parameters.Add(component => component.Id, 1));
        AssertAlert(recreated, "Scar");
    }

    [Fact]
    public void ShowAlert_ForFamilyMember_ShowsFamilyMemberNameInConfirmation()
    {
        var child = CreateChild();
        _familyStateService.Setup(service => service.GetChild(1)).Returns(child);
        var familyMember = new FamilyMember { Id = 4, GivenName = "Jamie", FamilyName = "Lee" };
        child.FamilyMembers.Add(familyMember);
        var cut = RenderComponent<ChildFamilyMembers>(parameters => parameters.Add(component => component.Id, 1));
        cut.Instance.ShowAlert(familyMember);
        cut.Render();

        var pageState = Services.GetRequiredService<IPageState>();
        pageState.AppSuspended = true;
        cut.Dispose();
        var recreated = RenderComponent<ChildFamilyMembers>(parameters => parameters.Add(component => component.Id, 1));
        AssertAlert(recreated, "Jamie Lee");
    }

    [Fact]
    public void ShowAlert_ForFriend_ShowsFriendNameInConfirmation()
    {
        var child = CreateChild();
        _familyStateService.Setup(service => service.GetChild(1)).Returns(child);
        var friend = new Person { Id = 5, GivenName = "Riley", FamilyName = "Smith" };
        child.Friends.Add(friend);
        var cut = RenderComponent<ChildFriends>(parameters => parameters.Add(component => component.Id, 1));
        cut.Instance.ShowAlert(friend);
        cut.Render();

        var pageState = Services.GetRequiredService<IPageState>();
        pageState.AppSuspended = true;
        cut.Dispose();
        var recreated = RenderComponent<ChildFriends>(parameters => parameters.Add(component => component.Id, 1));
        AssertAlert(recreated, "Riley Smith");
    }

    [Fact]
    public void ShowAlert_ForSocialMediaAccount_ShowsPlatformInConfirmation()
    {
        var child = CreateChild();
        _familyStateService.Setup(service => service.GetChild(1)).Returns(child);
        var account = new SocialMediaAccount { Id = 6, Platform = "Instagram" };
        child.SocialMediaAccounts.Add(account);
        var cut = RenderComponent<SocialMediaAccountsPage>(parameters => parameters.Add(component => component.Id, 1));
        cut.Instance.ShowAlert(account);
        cut.Render();

        var pageState = Services.GetRequiredService<IPageState>();
        pageState.AppSuspended = true;
        cut.Dispose();
        var recreated = RenderComponent<SocialMediaAccountsPage>(parameters => parameters.Add(component => component.Id, 1));
        AssertAlert(recreated, "Instagram");
    }

    private static ChildRecord CreateChild() => new() { Id = 1 };

    private static void AssertAlert<TComponent>(IRenderedComponent<TComponent> cut, string itemName)
        where TComponent : IComponent
    {
        var alert = cut.FindComponent<McmAlert>().Instance;

        Assert.Equal(ExpectedHeader, alert.Header);
        Assert.Equal(string.Format(ExpectedMessage, itemName), alert.Message);
        Assert.True(alert.Show);
    }
}
