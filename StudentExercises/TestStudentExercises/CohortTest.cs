using System.Net;
using Newtonsoft.Json;
using Xunit;
using StudentExercises.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace TestStudentExercises
{
    public class TestCohort
    {
        [Fact]
        public async Task Test_Get_All_Cohorts()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/cohorts");


                string responseBody = await response.Content.ReadAsStringAsync();
                var cohorts = JsonConvert.DeserializeObject<List<Cohort>>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(cohorts.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Get_One_Cohort()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var responseWithAllCohorts = await client.GetAsync("/api/cohorts");


                string responseBodyWithAllCohorts = await responseWithAllCohorts.Content.ReadAsStringAsync();
                var allCohorts = JsonConvert.DeserializeObject<List<Cohort>>(responseBodyWithAllCohorts);


                var response = await client.GetAsync("/api/cohorts/" + allCohorts.First().Id);

                string responseBody = await response.Content.ReadAsStringAsync();
                var cohort = JsonConvert.DeserializeObject<Cohort>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(cohort.Id > 0);
            }
        }

        [Fact]
        public async Task Test_Add_One_Cohort()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */

                var newCohort = new Cohort
                {
                    IsDayTime = true,
                    CohortNum = 35,
                };

                var newCohortAsJSON = JsonConvert.SerializeObject(newCohort);

                /*
                    ACT
                */
                var response = await client.PostAsync(
                    "/api/cohorts",
                    new StringContent(newCohortAsJSON, Encoding.UTF8, "application/json")
                );

                string responseBody = await response.Content.ReadAsStringAsync();

                var newCohortReturned = JsonConvert.DeserializeObject<Cohort>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(newCohortReturned.Id > 0);
            }
        }

        [Fact]
        public async Task Test_Update_One_Cohort()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */

                int newCohrt = 40;

                var newCohort = new Cohort
                {
                    IsDayTime = true,
                    CohortNum = newCohrt
                };

                var newCohortAsJSON = JsonConvert.SerializeObject(newCohort);

                /*
                    ACT
                */
                var response = await client.PutAsync(
                    "/api/cohorts/2",
                    new StringContent(newCohortAsJSON, Encoding.UTF8, "application/json")
                );

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Delete_One_Cohort()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */

                /*
                    ACT
                */
                var response = await client.DeleteAsync(
                    "/api/cohorts/5"
                );

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

    }
}
