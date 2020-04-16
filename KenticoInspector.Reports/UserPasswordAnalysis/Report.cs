using System.Collections.Generic;
using System.Linq;

using KenticoInspector.Core.Instances.Services;
using KenticoInspector.Core.Modules;
using KenticoInspector.Core.Modules.Models.Results;
using KenticoInspector.Core.Modules.Models.Results.Data;
using KenticoInspector.Modules;
using KenticoInspector.Reports.UserPasswordAnalysis.Models;
using KenticoInspector.Reports.UserPasswordAnalysis.Models.Data;
using KenticoInspector.Reports.UserPasswordAnalysis.Models.Results;

using static KenticoInspector.Core.Modules.Models.Tags;

namespace KenticoInspector.Reports.UserPasswordAnalysis
{
    public class Report : AbstractReport<Terms>
    {
        private readonly IDatabaseService databaseService;

        public static IEnumerable<string> ExcludedUserNames => new List<string>
        {
            "public"
        };

        public Report(IDatabaseService databaseService)
        {
            this.databaseService = databaseService;
        }

        [Tags(
            Security,
            Users
            )]
        [SupportsVersions("10 - 12.0")]
        public override ReportResults GetResults()
        {
            var users = databaseService.ExecuteSqlFromFile<CmsUser>(
                Scripts.GetEnabledAndNotExternalUsers,
                new
                {
                    ExcludedUserNames
                }
                );

            var usersWithEmptyPasswords = GetUsersWithEmptyPasswords(users);
            var usersWithPlaintextPasswords = GetUsersWithPlaintextPasswords(users);

            return CompileResults(
                usersWithEmptyPasswords,
                usersWithPlaintextPasswords
                );
        }

        private static IEnumerable<CmsUserResultWithPasswordFormat> GetUsersWithEmptyPasswords(
            IEnumerable<CmsUser> users
            )
        {
            return users
                .Where(user => string.IsNullOrEmpty(user.UserPassword))
                .Select(user => new CmsUserResultWithPasswordFormat(user));
        }

        private static IEnumerable<CmsUserResult> GetUsersWithPlaintextPasswords(IEnumerable<CmsUser> users)
        {
            return users
                .Where(user => string.IsNullOrEmpty(user.UserPasswordFormat))
                .Select(user => new CmsUserResult(user));
        }

        private ReportResults CompileResults(
            IEnumerable<CmsUserResultWithPasswordFormat> usersWithEmptyPasswords,
            IEnumerable<CmsUserResult> usersWithPlaintextPasswords
            )
        {
            if (!usersWithEmptyPasswords.Any() && !usersWithPlaintextPasswords.Any())
                return new ReportResults(ResultsStatus.Good)
                {
                    Summary = Metadata.Terms.GoodSummary
                };

            var emptyCount = usersWithEmptyPasswords.Count();
            var plaintextCount = usersWithPlaintextPasswords.Count();

            return new ReportResults(ResultsStatus.Error)
            {
                Summary = Metadata.Terms.ErrorSummary.With(
                    new
                    {
                        emptyCount,
                        plaintextCount
                    }
                    ),
                Data =
                {
                    usersWithEmptyPasswords.AsResult()
                        .WithLabel(Metadata.Terms.TableTitles.EmptyPasswords),
                    usersWithPlaintextPasswords.AsResult()
                        .WithLabel(Metadata.Terms.TableTitles.PlaintextPasswords)
                }
            };
        }
    }
}