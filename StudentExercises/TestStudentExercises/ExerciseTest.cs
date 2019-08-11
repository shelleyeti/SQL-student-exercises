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
    public class TestExercise
    {
        [Fact]
        public async Task Test_Get_All_Exercises()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/exercises");


                string responseBody = await response.Content.ReadAsStringAsync();
                var exercises = JsonConvert.DeserializeObject<List<Exercise>>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(exercises.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Get_One_Exercise()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var responseWithAllExercises = await client.GetAsync("/api/exercises");


                string responseBodyWithAllExercises = await responseWithAllExercises.Content.ReadAsStringAsync();
                var allExercises = JsonConvert.DeserializeObject<List<Exercise>>(responseBodyWithAllExercises);


                var response = await client.GetAsync("/api/exercises/" + allExercises.First().Id);

                string responseBody = await response.Content.ReadAsStringAsync();
                var exercise = JsonConvert.DeserializeObject<Exercise>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(exercise.Id > 0);
            }
        }

        [Fact]
        public async Task Test_Add_One_Exercise()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */

                var newExercise = new Exercise
                {
                    ExName = "Testing",
                    ExLanguage = "C#"
                };

                var newExerciseAsJSON = JsonConvert.SerializeObject(newExercise);

                /*
                    ACT
                */
                var response = await client.PostAsync(
                    "/api/exercises",
                    new StringContent(newExerciseAsJSON, Encoding.UTF8, "application/json")
                );


                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Update_One_Exercise()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */

                string newName = "fingers bleed";

                var newExercise = new Exercise
                {
                    ExName = newName,
                    ExLanguage = "C#"
                };

                var newExerciseAsJSON = JsonConvert.SerializeObject(newExercise);

                /*
                    ACT
                */
                var response = await client.PutAsync(
                    "/api/exercises/8",
                    new StringContent(newExerciseAsJSON, Encoding.UTF8, "application/json")
                );

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Delete_One_Exercise()
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
                    "/api/exercises/6"
                );

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

    }
}
