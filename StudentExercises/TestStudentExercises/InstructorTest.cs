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
    public class TestInstructor
    {
        [Fact]
        public async Task Test_Get_All_Instructors()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/instructors");


                string responseBody = await response.Content.ReadAsStringAsync();
                var instructors = JsonConvert.DeserializeObject<List<Instructor>>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(instructors.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Get_One_Instructor()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var responseWithAllInstructors = await client.GetAsync("/api/instructors");


                string responseBodyWithAllInstructors = await responseWithAllInstructors.Content.ReadAsStringAsync();
                var allInstructors = JsonConvert.DeserializeObject<List<Instructor>>(responseBodyWithAllInstructors);


                var response = await client.GetAsync("/api/instructors/" + allInstructors.First().Id);

                string responseBody = await response.Content.ReadAsStringAsync();
                var instructor = JsonConvert.DeserializeObject<Instructor>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(instructor.Id > 0);
            }
        }

        [Fact]
        public async Task Test_Add_One_Instructor()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */

                var newInstructor = new Instructor
                {
                    FirstName = "Cavy",
                    LastName = "Baby",
                    SlackHandle = "cavy-baby",
                    cohort = new Cohort { CohortNum = 32 }
                };

                var newInstructorAsJSON = JsonConvert.SerializeObject(newInstructor);

                /*
                    ACT
                */
                var response = await client.PostAsync(
                    "/api/instructors",
                    new StringContent(newInstructorAsJSON, Encoding.UTF8, "application/json")
                );

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Update_One_Instructor()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */

                string newHandle = "woof-woof";

                var newInstructor = new Instructor
                {
                    FirstName = "Cavy",
                    LastName = "Baby",
                    SlackHandle = newHandle
                };

                var newInstructorAsJSON = JsonConvert.SerializeObject(newInstructor);

                /*
                    ACT
                */
                var response = await client.PutAsync(
                    "/api/instructors/5",
                    new StringContent(newInstructorAsJSON, Encoding.UTF8, "application/json")
                );

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Delete_One_Instructor()
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
                    "/api/instructors/6"
                );

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

    }
}
