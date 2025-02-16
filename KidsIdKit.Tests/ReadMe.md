Notes regarding bUnit tests:

    - bUnit is a testing library for Blazor components

    - to run all the tests in Visual Studio:
        - select "Run All Tests" from the Test menu or
        - press [Ctrl+R, A]
      To see the results of the tests, open the Test Explorer window by selecting "Test" from the top menu and then "Test Explorer".

    - within the tests themselves:

        - RenderComponent<TComponent>() is a more straightforward and commonly used approach in bUnit for rendering components while ...
        - Render(@<Amberalert />) uses Razor syntax to achieve the same result
