using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using HostelFinder.Application.DTOs.ChatAI.Request;
using HostelFinder.Application.DTOs.ChatAI.Response;
using HostelFinder.Application.DTOs.Post.Requests;
using HostelFinder.Application.DTOs.Post.Responses;
using HostelFinder.Application.Interfaces.IRepositories;
using HostelFinder.Application.Interfaces.IServices;
using Microsoft.Extensions.Configuration;

namespace HostelFinder.Application.Services;

public class OpenAiService : IOpenAiService
{
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly IHostelRepository _hostelRepository;
    private readonly IRoomRepository _roomRepository;

    public OpenAiService(IConfiguration configuration, IHttpClientFactory httpClientFactory, IHostelRepository hostelRepository, IRoomRepository roomRepository)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        _hostelRepository = hostelRepository;
        _roomRepository = roomRepository;
    }
   
    public async Task<OpenAiChatResponse> GenerateAsync(OpenAiChatRequest request)
    {
        if (request == null || request.Messages == null || request.Messages.Count == 0)
        {
            throw new ArgumentException("Messages are required.");
        }

        var apiKey = _configuration["OpenAI:ApiKey"];
        if (string.IsNullOrEmpty(apiKey))
        {
            throw new InvalidOperationException("OpenAI API Key is not configured.");
        }

        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        var apiUrl = "https://api.openai.com/v1/chat/completions";

        var payload = new
        {
            model = request.Model, // "gpt-3.5-turbo" hoặc "gpt-4"
            messages = request.Messages,
            temperature = request.Temperature
        };

        var content = new StringContent(JsonSerializer.Serialize(payload, _jsonOptions), Encoding.UTF8, "application/json");

        var response = await client.PostAsync(apiUrl, content);
        var responseString = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Error calling OpenAI API: {responseString}");
        }

        var openAiResponse = JsonSerializer.Deserialize<OpenAiChatResponse>(responseString, _jsonOptions);
        return openAiResponse;
    }

    public async Task<PostGenerationResponse> GeneratePostDescriptionsAsync(PostGenerationRequest request)
    {
        try
        {
            var room = await _roomRepository.GetRoomByIdAsync(request.RoomId);
            var hostel = await _hostelRepository.GetHostelByIdAsync(request.HostelId);
            var promptTitle = $"Tạo một tiêu đề hấp dẫn cho bài đăng cho thuê phòng trọ với các thông tin sau và chú ý kết quả chỉ hiển thị ra tiêu đề:\n" +
                                    $"Tên nhà trọ: {hostel.HostelName}\n" +
                                    $"Địa chỉ: {hostel.Address.DetailAddress}, {hostel.Address.Commune}, {hostel.Address.District}, {hostel.Address.Province}\n" +
                                    $"Loại phòng: {room.RoomType}\n" +
                                    $"Diện tích: {room.Size}m²\n" +
                                    $"Tầng: {room.Floor}\n" +
                                    $"Giá: {room.MonthlyRentCost} VND\n";
            var promptDescription = $"Viết một mô tả chi tiết cho bài đăng cho thuê phòng trọ hấp dẫn, sinh động và" +
                                    $" mô tả vị trí thuận tiện gần các trường đại học hoặc gần các vị trí trọng điểm, trung tâm nào " +
                                    $"thêm các icon cho người dùng dễ nhìn, hiện thị các ý rõ ràng cho người thuê, " +
                                    $"chú ý tối đa 3000 kí tự và kết quả trả về chỉ là mô tả chi tiết về phòng trọ cho thuê " +
                                    $"với các thông tin sau :\n" +
                         $"Tên nhà trọ: {hostel.HostelName}\n" +
                         $"Địa chỉ: {hostel.Address.DetailAddress}, {hostel.Address.Commune}, {hostel.Address.District}, {hostel.Address.Province}\n" +
                         $"Tên phòng: {room.RoomName}\n" +
                         $"Loại phòng: {room.RoomType}\n" +
                         $"Diện tích: {room.Size}m²\n" +
                         $"Tầng: {room.Floor}\n" +
                         $"Số người phù hợp: {room.MaxRenters}\n";
            var requestTitleOpenAi = new OpenAiChatRequest
            {
                Model = "gpt-4",
                Temperature = 0.7,
                Messages = new List<Message>
                {
                    new Message
                    {
                        Role = "system",
                        Content = "Bạn là một chuyên gia viết tiêu đề quảng cáo cho các bài đăng cho thuê phòng trọ."
                    },
                    new Message
                    {
                        Role = "user",
                        Content = promptTitle
                    }
                }
            };
            var requestDescriptionOpenAi = new OpenAiChatRequest
            {
                Model = "gpt-4",
                Temperature = 0.7,
                Messages = new List<Message>
                {
                    new Message
                    {
                        Role = "system",
                        Content = "Bạn là một chuyên gia viết tiêu đề quảng cáo cho các bài đăng cho thuê phòng trọ."
                    },
                    new Message
                    {
                        Role = "user",
                        Content = promptDescription
                    }
                }
            };
            
            var responseTitle = await GenerateAsync(requestTitleOpenAi);
            var postTitle =  responseTitle.Choices.FirstOrDefault()?.Message?.Content.Trim('"');

            var responseDescription = await GenerateAsync(requestDescriptionOpenAi);
            var postDescription = responseDescription.Choices.LastOrDefault()?.Message?.Content.Trim();
            return new PostGenerationResponse
            {
                Title = postTitle,
                Description = postDescription
            };
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}