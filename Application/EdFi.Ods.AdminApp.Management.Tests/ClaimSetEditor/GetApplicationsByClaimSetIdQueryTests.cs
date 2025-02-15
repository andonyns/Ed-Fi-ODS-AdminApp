// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Admin.DataAccess.Contexts;
using NUnit.Framework;
using Shouldly;
using EdFi.Ods.AdminApp.Management.ClaimSetEditor;
using Application = EdFi.Security.DataAccess.Models.Application;
using VendorApplication = EdFi.Admin.DataAccess.Models.Application;
using ClaimSet = EdFi.Security.DataAccess.Models.ClaimSet;
using static EdFi.Ods.AdminApp.Management.Tests.Testing;

namespace EdFi.Ods.AdminApp.Management.Tests.ClaimSetEditor
{
    [TestFixture]
    public class GetApplicationsByClaimSetIdQueryTests : SecurityDataTestBase
    {
        [TestCase(1)]
        [TestCase(3)]
        [TestCase(5)]
        public void ShouldGetApplicationsByClaimSetId(int applicationCount)
        {
            var testClaimSets = SetupApplicationWithClaimSets();

            SetupApplications(testClaimSets, applicationCount);

            foreach (var testClaimSet in testClaimSets)
            {
                var results = Scoped<IGetApplicationsByClaimSetIdQuery, Management.ClaimSetEditor.Application[]>(
                    query => query.Execute(testClaimSet.ClaimSetId).ToArray());

                Scoped<IUsersContext>(usersContext =>
                {
                    var testApplications =
                        usersContext.Applications.Where(x => x.ClaimSetName == testClaimSet.ClaimSetName).ToArray();
                    results.Length.ShouldBe(testApplications.Length);
                    results.Select(x => x.Name).ShouldBe(testApplications.Select(x => x.ApplicationName), true);
                });
            }
        }


        [TestCase(1)]
        [TestCase(5)]
        public void ShouldGetClaimSetApplicationsCount(int applicationsCount)
        {
            var testClaimSets = SetupApplicationWithClaimSets();

            SetupApplications(testClaimSets, applicationsCount);

            foreach (var testClaimSet in testClaimSets)
            {
                var appsCountByClaimSet = Scoped<IGetApplicationsByClaimSetIdQuery, int>(
                    query => query.ExecuteCount(testClaimSet.ClaimSetId));

                Scoped<IUsersContext>(usersContext =>
                {
                    var testApplicationsCount =
                        usersContext.Applications.Count(x => x.ClaimSetName == testClaimSet.ClaimSetName);
                    appsCountByClaimSet.ShouldBe(testApplicationsCount);
                });
            }
        }

        private IReadOnlyCollection<ClaimSet> SetupApplicationWithClaimSets(
            string applicationName = "TestApplicationName", int claimSetCount = 5)
        {
            var testApplication = new Application
            {
                ApplicationName = applicationName
            };

            Save(testApplication);

            var testClaimSetNames = Enumerable.Range(1, claimSetCount)
                .Select((x, index) => $"TestClaimSetName{index:N}")
                .ToArray();

            var testClaimSets = testClaimSetNames
                .Select(x => new ClaimSet
                {
                    ClaimSetName = x,
                    Application = testApplication
                })
                .ToArray();

            Save(testClaimSets.Cast<object>().ToArray());

            return testClaimSets;
        }

        private static void SetupApplications(IEnumerable<ClaimSet> testClaimSets, int applicationCount = 5)
        {
            Scoped<IUsersContext>(usersContext =>
            {
                foreach (var claimSet in testClaimSets)
                {
                    foreach (var _ in Enumerable.Range(1, applicationCount))
                    {
                        usersContext.Applications.Add(new VendorApplication
                        {
                            ApplicationName = $"TestAppVendorName{Guid.NewGuid():N}",
                            ClaimSetName = claimSet.ClaimSetName,
                            OperationalContextUri = OperationalContext.DefaultOperationalContextUri
                        });
                    }
                }
                usersContext.SaveChanges();
            });
        }
    }
}
