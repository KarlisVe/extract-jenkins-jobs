using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace jenkins_extract_jobs
{
    static class Program
    {
        private static List<StructJenkinsJobs> _jenkinsJobs = new List<StructJenkinsJobs>();

        private static void AddJobToList(StructJenkinsJobs job)
        {
            _jenkinsJobs.Add(job);
        }

        private static async Task Main(string[] args)
        {
            var username = args[0];
            var password = args[1];
            var urlJenkins = args[2];
            var folder = "root"; //jobs which are in root listing

            var auth = new AuthenticationHeaderValue(
                "Basic", Convert.ToBase64String(
                    System.Text.Encoding.ASCII.GetBytes(
                        $"{username}:{password}")));

            await AddFolderJobs(folder, urlJenkins, auth);

            foreach (var jobEntity in _jenkinsJobs)
            {
                Console.WriteLine($"{jobEntity.Url} / {jobEntity.Folder} / {jobEntity.Name}");
            }
        }

        private record StructJenkinsJobs
        {
            public string Folder { get; set; }
            public string Name { get; set; }
            public string Url { get; set; }
        }

        private static async Task AddFolderJobs(string folder, string url, AuthenticationHeaderValue auth)
        {
            url = $"{url}api/json";
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = auth;
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            using var jenkinsJson =
                await JsonDocument.ParseAsync(await httpClient.GetStreamAsync(url));

            JsonElement jsonRoot = jenkinsJson.RootElement;

            foreach (var job in jsonRoot.GetProperty("jobs").EnumerateArray())
            {
                if (job.GetProperty("_class").ToString().Equals("com.cloudbees.hudson.plugins.folder.Folder",
                    StringComparison.OrdinalIgnoreCase))
                {
                    await AddFolderJobs(job.GetProperty("name").ToString(), job.GetProperty("url").ToString(), auth);
                    continue;
                }

                var jobEntity = new StructJenkinsJobs
                {
                    Folder = folder, Name = job.GetProperty("name").ToString(),
                    Url = job.GetProperty("url").ToString()
                };
                AddJobToList(jobEntity);
            }
        }
    }
}