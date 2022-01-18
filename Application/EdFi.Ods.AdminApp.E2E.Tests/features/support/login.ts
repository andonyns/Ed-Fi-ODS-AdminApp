import { Given, Then, When } from "@cucumber/cucumber";
import { ok } from "assert";
import { takeScreenshot } from "../management/functions";
import { models } from "../management/setup";
import { validatePath } from "../management/validators";

Given("there are no users registered", async () => {
    ok(await models.loginPage.hasRegisterButton(), "There is a user already registered");
});

Given("user is registered", async () => {
    await models.loginPage.navigate();
    if (!(await models.loginPage.hasRegisterButton())) {
        //Registration not required
        return;
    }
    await models.loginPage.fullRegistration(process.env.email, process.env.password);
});

When("clicking on register as a new user", async () => {
    await models.loginPage.startRegistration();
});

When("user enters valid email and password", async () => {
    await models.loginPage.fillForm(process.env.email, process.env.password);
});

When("password confirmation", async () => {
    await models.loginPage.fillPasswordConfirm(process.env.password);
});

When("clicks Log in", async () => {
    await models.loginPage.login();
});

When("clicks Register", async () => {
    await models.loginPage.register();
});

Then("login is successful", async () => {
    if (models.homePage.isOnPage) {
        ok(await models.homePage.hasGlobalOption());
        ok(await models.homePage.hasSettingsOption());
    } else if (models.firstTimeSetupPage.isOnPage) {
        ok(await models.firstTimeSetupPage.hasTitle());
    } else {
        throw "Login failed";
    }

    await takeScreenshot("login successful");
});

Then("registration is successful", async () => {
    validatePath(models.firstTimeSetupPage.path(), true);
    ok(await models.firstTimeSetupPage.hasTitle());
    await takeScreenshot("registration successful");
});
