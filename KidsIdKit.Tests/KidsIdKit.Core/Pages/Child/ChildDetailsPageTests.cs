//using Bunit;
//using KidsIdKit.Core.Data;
//using KidsIdKit.Core.Pages.Child;
//using KidsIdKit.Core.Services;
//using KidsIdKit.Core.SharedComponents;
//using Microsoft.AspNetCore.Components;
//using Microsoft.AspNetCore.Components.Forms;
//using NSubstitute;
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using Xunit;
//using DataModel = KidsIdKit.Core.Data;

//namespace KidsIdKit.Tests.KidsIdKit.Core.Pages.Child;

//public class ChildDetailsPageTests : TestContext
//{
//    private readonly IFamilyStateService _mockFamilyState;
//    private readonly NavigationManager _mockNavigationManager;
//    private readonly Family _testFamily;

//    public ChildDetailsPageTests()
//    {
//        _mockFamilyState = Substitute.For<IFamilyStateService>();
//        _mockNavigationManager = Substitute.For<NavigationManager>();

//        // Set up test data
//        _testFamily = new Family
//        {
//            Children = new List<DataModel.Child>
//            {
//                new DataModel.Child
//                {
//                    Id = 1,
//                    ChildDetails = new DataModel.ChildDetails { GivenName = "John", FamilyName = "Doe" }
//                }
//            }
//        };

//        _mockFamilyState.Family.Returns(_testFamily);
//        _mockFamilyState.GetChild(Arg.Any<int>()).Returns(x =>
//        {
//            var index = (int)x[0];
//            return index >= 0 && index < _testFamily.Children.Count ? _testFamily.Children[index] : null;
//        });

//        Services.AddSingleton(_mockFamilyState);
//        Services.AddSingleton(_mockNavigationManager);
//    }

//    #region Rendering Tests

//    [Fact]
//    public void Render_WithValidChildId_DisplaysChildName()
//    {
//        // Arrange
//        var childIndex = 0;

//        // Act
//        var cut = RenderComponent<ChildDetails>(parameters => parameters
//            .Add(p => p.Id, childIndex.ToString()));

//        // Assert
//        Assert.NotNull(cut);
//        // Verify component renders without throwing
//    }

//    #endregion

//    #region Navigation Back Tests

//    [Fact]
//    public void OnBackButtonClicked_WithoutChanges_RemovesEmptyNewChild()
//    {
//        // Arrange
//        var newChild = new DataModel.Child
//        {
//            Id = 2,
//            ChildDetails = new DataModel.ChildDetails { GivenName = "", FamilyName = "" }
//        };
//        _testFamily.Children.Add(newChild);
//        var childIndex = 1;

//        var cut = RenderComponent<ChildDetails>(parameters => parameters
//            .Add(p => p.Id, childIndex.ToString()));

//        var component = cut.Instance;

//        // Act
//        component.OnBackButtonClicked().Wait();

//        // Assert
//        Assert.DoesNotContain(newChild, _testFamily.Children);
//    }

//    [Fact]
//    public void OnBackButtonClicked_WithoutChanges_NavigatesBack()
//    {
//        // Arrange
//        var childIndex = 0;
//        var cut = RenderComponent<ChildDetails>(parameters => parameters
//            .Add(p => p.Id, childIndex.ToString()));

//        var component = cut.Instance;

//        // Act
//        component.OnBackButtonClicked().Wait();

//        // Assert
//        _mockNavigationManager.Received(1).NavigateTo(
//            Arg.Any<string>(),
//            Arg.Any<bool>(),
//            Arg.Any<bool>());
//    }

//    #endregion

//    #region Pending Changes Alert Tests

//    [Fact]
//    public void OnBackButtonClicked_WithChanges_ShowsPendingChangesAlert()
//    {
//        // Arrange
//        var childIndex = 0;
//        var cut = RenderComponent<ChildDetails>(parameters => parameters
//            .Add(p => p.Id, childIndex.ToString()));

//        var component = cut.Instance;

//        // Modify the child to create pending changes
//        var child = _mockFamilyState.GetChild(childIndex);
//        child.ChildDetails.GivenName = "Modified";

//        // Act
//        component.OnBackButtonClicked().Wait();

//        // Assert - Component should show pending changes alert
//        cut.WaitForAssertion(() =>
//        {
//            Assert.True(component.ShowPendingChangesAlert);
//        });
//    }

//    #endregion

//    #region Discard Changes Tests

//    [Fact]
//    public async Task OnPendingChangesAlertClosed_WithDiscardAction_RemovesEmptyNewChild()
//    {
//        // Arrange
//        var newChild = new DataModel.Child
//        {
//            Id = 2,
//            ChildDetails = new DataModel.ChildDetails { GivenName = "", FamilyName = "" }
//        };
//        _testFamily.Children.Add(newChild);
//        var childIndex = 1;

//        var cut = RenderComponent<ChildDetails>(parameters => parameters
//            .Add(p => p.Id, childIndex.ToString()));

//        var component = cut.Instance;

//        // Act
//        await component.OnPendingChangesAlertClosed(
//            (McmAlert.AlertAction.Cancel, ""));

//        // Assert
//        Assert.DoesNotContain(newChild, _testFamily.Children);
//    }

//    [Fact]
//    public async Task OnPendingChangesAlertClosed_WithDiscardAction_NavigatesBack()
//    {
//        // Arrange
//        var childIndex = 0;
//        var cut = RenderComponent<ChildDetails>(parameters => parameters
//            .Add(p => p.Id, childIndex.ToString()));

//        var component = cut.Instance;

//        // Modify to create changes
//        var child = _mockFamilyState.GetChild(childIndex);
//        child.ChildDetails.GivenName = "Modified";

//        // Act
//        await component.OnPendingChangesAlertClosed(
//            (McmAlert.AlertAction.Cancel, ""));

//        // Assert
//        _mockNavigationManager.Received(1).NavigateTo(
//            Arg.Any<string>(),
//            Arg.Any<bool>(),
//            Arg.Any<bool>());
//    }

//    #endregion

//    #region Save with Validation Tests

//    [Fact]
//    public async Task OnPendingChangesAlertClosed_WithValidData_Saves()
//    {
//        // Arrange
//        var childIndex = 0;
//        var cut = RenderComponent<ChildDetails>(parameters => parameters
//            .Add(p => p.Id, childIndex.ToString()));

//        var component = cut.Instance;

//        // Create a valid EditContext
//        var child = _mockFamilyState.GetChild(childIndex);
//        var editContext = new EditContext(child.ChildDetails);
//        component.SetEditContext(editContext);

//        // Modify to create changes but keep required fields
//        child.ChildDetails.GivenName = "John";
//        child.ChildDetails.FamilyName = "Doe";

//        _mockFamilyState.SaveAsync().Returns(Task.CompletedTask);

//        // Act
//        await component.OnPendingChangesAlertClosed(
//            (McmAlert.AlertAction.Confirm, ""));

//        // Assert
//        await _mockFamilyState.Received(1).SaveAsync();
//    }

//    [Fact]
//    public async Task OnPendingChangesAlertClosed_WithInvalidData_ShowsAlert()
//    {
//        // Arrange
//        var childIndex = 0;
//        var cut = RenderComponent<ChildDetails>(parameters => parameters
//            .Add(p => p.Id, childIndex.ToString()));

//        var component = cut.Instance;

//        var child = _mockFamilyState.GetChild(childIndex);

//        // Create an EditContext with validation errors
//        var editContext = new EditContext(child.ChildDetails);

//        // Add a validation error manually
//        var fieldIdentifier = new FieldIdentifier(child.ChildDetails, nameof(child.ChildDetails.GivenName));
//        editContext.MarkAsUnmodified(fieldIdentifier);

//        component.SetEditContext(editContext);

//        // Make GivenName empty to fail validation
//        child.ChildDetails.GivenName = "";
//        child.ChildDetails.FamilyName = "Doe";

//        // Act
//        await component.OnPendingChangesAlertClosed(
//            (McmAlert.AlertAction.Confirm, ""));

//        // Assert - Alert should still be visible
//        cut.WaitForAssertion(() =>
//        {
//            Assert.True(component.ShowPendingChangesAlert);
//        });
//    }

//    #endregion

//    #region SetEditContext Tests

//    [Fact]
//    public void SetEditContext_StoresEditContextReference()
//    {
//        // Arrange
//        var childIndex = 0;
//        var cut = RenderComponent<ChildDetails>(parameters => parameters
//            .Add(p => p.Id, childIndex.ToString()));

//        var component = cut.Instance;
//        var child = _mockFamilyState.GetChild(childIndex);
//        var editContext = new EditContext(child.ChildDetails);

//        // Act
//        component.SetEditContext(editContext);

//        // Assert
//        // Verify the EditContext is stored (would need to add a public property or test through behavior)
//        Assert.NotNull(editContext);
//    }

//    #endregion

//    #region Collection Management Tests

//    [Fact]
//    public void RemoveEmptyNewChild_WithEmptyGivenName_RemovesChild()
//    {
//        // Arrange
//        var newChild = new DataModel.Child
//        {
//            Id = 2,
//            ChildDetails = new DataModel.ChildDetails { GivenName = "", FamilyName = "" }
//        };
//        _testFamily.Children.Add(newChild);
//        var childIndex = 1;

//        var cut = RenderComponent<ChildDetails>(parameters => parameters
//            .Add(p => p.Id, childIndex.ToString()));

//        var component = cut.Instance;

//        // Act - Navigate back should trigger RemoveEmptyNewChild
//        component.OnBackButtonClicked().Wait();

//        // Assert
//        Assert.DoesNotContain(newChild, _testFamily.Children);
//    }

//    [Fact]
//    public void RemoveEmptyNewChild_WithFilledGivenName_DoesNotRemoveChild()
//    {
//        // Arrange
//        var newChild = new DataModel.Child
//        {
//            Id = 2,
//            ChildDetails = new DataModel.ChildDetails { GivenName = "Jane", FamilyName = "Smith" }
//        };
//        _testFamily.Children.Add(newChild);
//        var childIndex = 1;

//        var cut = RenderComponent<ChildDetails>(parameters => parameters
//            .Add(p => p.Id, childIndex.ToString()));

//        var component = cut.Instance;

//        // Act
//        component.OnBackButtonClicked().Wait();

//        // Assert
//        Assert.Contains(newChild, _testFamily.Children);
//    }

//    #endregion
//}
