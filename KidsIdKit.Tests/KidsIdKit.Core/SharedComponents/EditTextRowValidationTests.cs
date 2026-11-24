using System;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Bunit;
using KidsIdKit.Core.SharedComponents;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;

namespace KidsIdKit.Tests.KidsIdKit.Core.SharedComponents;

public class EditTextRowValidationTests : TestContext
{
    [Fact]
    public async Task RequiredField_IsOutlinedWhenInvalid_AndOutlineClearsWhenCorrected()
    {
        var model = new FormModel();
        var editContext = new EditContext(model);
        var cut = RenderComponent<CascadingValue<EditContext>>(parameters => parameters
            .Add(component => component.Value, editContext)
            .Add(component => component.ChildContent, (RenderFragment)(builder =>
            {
                builder.OpenComponent<DataAnnotationsValidator>(0);
                builder.CloseComponent();

                builder.OpenComponent<EditTextRow>(1);
                builder.AddAttribute(2, "Value", model.Name);
                builder.AddAttribute(3, "ValueChanged", EventCallback.Factory.Create<string>(this, value => model.Name = value));
                builder.AddAttribute(4, "ValueExpression", (Expression<Func<string?>>)(() => model.Name));
                builder.CloseComponent();
            })));

        Assert.Single(cut.FindAll("ion-input"));
        Assert.DoesNotContain("field-validation-error", cut.Find(".field-validation-wrapper").ClassList);

        var isValid = await cut.InvokeAsync(editContext.Validate);
        Assert.False(isValid);
        cut.WaitForAssertion(() => Assert.Contains("field-validation-error", cut.Find(".field-validation-wrapper").ClassList));
        Assert.Single(cut.FindAll("ion-input"));

        await cut.InvokeAsync(() => model.Name = "Alex");
        isValid = await cut.InvokeAsync(editContext.Validate);
        Assert.True(isValid);
        cut.WaitForAssertion(() => Assert.DoesNotContain("field-validation-error", cut.Find(".field-validation-wrapper").ClassList));
        Assert.Single(cut.FindAll("ion-input"));
    }

    private sealed class FormModel
    {
        [Required]
        public string? Name { get; set; }
    }
}
