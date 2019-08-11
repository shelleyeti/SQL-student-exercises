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
    public class TestStudent
    {
        [Fact]
        public async Task Test_Get_All_Students()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/students");


                string responseBody = await response.Content.ReadAsStringAsync();
                var students = JsonConvert.DeserializeObject<List<Student>>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(students.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Get_One_Student()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var responseWithAllStudents = await client.GetAsync("/api/students");


                string responseBodyWithAllStudents = await responseWithAllStudents.Content.ReadAsStringAsync();
                var allStudents = JsonConvert.DeserializeObject<List<Cohort>>(responseBodyWithAllStudents);


                var response = await client.GetAsync("/api/students/" + allStudents.First().Id);

                string responseBody = await response.Content.ReadAsStringAsync();
                var student = JsonConvert.DeserializeObject<Student>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(student.Id > 0);
            }
        }

        [Fact]
        public async Task Test_Add_One_Student()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */

                var newStudent = new Student
                {
                    FirstName = "ImOK",
                    LastName = "ThisIsFine",
                    SlackHandle = "tedious"
                };

                var newStudentAsJSON = JsonConvert.SerializeObject(newStudent);

                /*
                    ACT
                */
                var response = await client.PostAsync(
                    "/api/students",
                    new StringContent(newStudentAsJSON, Encoding.UTF8, "application/json")
                );

                string responseBody = await response.Content.ReadAsStringAsync();

                var newStudentReturned = JsonConvert.DeserializeObject<Student>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(newStudentReturned.Id > 0);
            }
        }

        [Fact]
        public async Task Test_Update_One_Student()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */

                string newName = "Cry_Face";

                var newStudent = new Student
                {
                    FirstName = newName,
                    LastName = "ThisIsFine",
                    SlackHandle = "tedious"
                };

                var newStudentAsJSON = JsonConvert.SerializeObject(newStudent);

                /*
                    ACT
                */
                var response = await client.PutAsync(
                    "/api/students/8",
                    new StringContent(newStudentAsJSON, Encoding.UTF8, "application/json")
                );

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Delete_One_Student()
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
                    "/api/students/6"
                );

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

    }
}
