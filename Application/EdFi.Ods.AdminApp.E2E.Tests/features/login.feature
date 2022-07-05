# SPDX-License-Identifier: Apache-2.0
# Licensed to the Ed-Fi Alliance under one or more agreements.
# The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
# See the LICENSE and NOTICES files in the project root for more information.

Feature: Log in

    Background: User is registered
        Given user is registered

    #AA-994
    @Sanity
    Scenario: Log in successful
        Given it's on the "Log in" page
        When user enters valid email and password
        And clicks Log in
        Then login is successful
