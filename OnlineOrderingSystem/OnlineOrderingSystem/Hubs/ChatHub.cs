using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using OnlineOrderingSystem.Data;
using OnlineOrderingSystem.Models;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.AspNetCore.Identity;
using System.Runtime.Intrinsics.X86;

public class ChatHub : Hub
{
    private readonly ApplicationDbContext _context;
    private static ConcurrentDictionary<string, string> OnlineUsers = new ConcurrentDictionary<string, string>();
    private readonly ILogger<ChatHub> _logger;
    private readonly UserManager<User> _userManager;

    public ChatHub(ApplicationDbContext context, ILogger<ChatHub> logger, UserManager<User> userManager)
    {
        _context = context;
        _logger = logger;
        _userManager = userManager;
    }

    public override async Task OnConnectedAsync()
    {
        string username = Context.GetHttpContext().Request.Query["username"];
        if (!string.IsNullOrEmpty(username))
        {
            OnlineUsers[Context.ConnectionId] = username;
            _logger.LogInformation($"User connected: {username}");

            await Clients.All.SendAsync("UpdateOnlineUsers", OnlineUsers.Values.Distinct().ToList());
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        if (OnlineUsers.TryRemove(Context.ConnectionId, out var username))
        {
            if (!string.IsNullOrEmpty(username))
            {
                _logger.LogInformation($"User disconnected: {username}");
            }
            await Clients.All.SendAsync("UpdateOnlineUsers", OnlineUsers.Values.Distinct().ToList());
        }
        await base.OnDisconnectedAsync(exception);
    }



    public async Task JoinGroup(int conversationId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, conversationId.ToString());
    }

    public async Task SendMessage(int conversationId, string userId, string message, string user2)
    {
        var userData = await _userManager.FindByNameAsync(userId);
        var userData2 = await _userManager.FindByNameAsync(user2);

        if (userData != null)
        {
            var chatMessage = new ChatMessage
            {
                Message = message,
                UserId = userData.Id,
                User = userData,
                ConversationId = conversationId,
                SendAt = DateTime.Now
            };

            _context.ChatMessages.Add(chatMessage);
            await _context.SaveChangesAsync();



            // Add current user to the group
          //  await Groups.AddToGroupAsync(userId, conversationId.ToString());
          //  await Groups.AddToGroupAsync(user2, conversationId.ToString());



            // Send message to all users in the conversation group
            //await Clients.Group(conversationId.ToString()).SendAsync("ReceiveMessage", userId, message);
            await Clients.User(userData2!.Id).SendAsync("ReceiveMessage", userId, message);
        }
    }

    public async Task Conversation(string user1, string user2)
    {
        var userData1 = await _userManager.FindByNameAsync(user1);

        var userData2 = await _userManager.FindByNameAsync(user2);


        var conversation = await _context.Conversations
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c =>
                (c.User1Id == userData1.Id && c.User2Id == userData2.Id) ||
                (c.User1Id == userData2.Id && c.User2Id == userData1.Id));

        if (conversation == null)
        {
            conversation = new Conversation
            {
                User1Id = userData1.Id,
                User1 = userData1,
                User2Id = userData2.Id,
                User2 = userData2,
                Messages = new List<ChatMessage>()
            };
            _context.Conversations.Add(conversation);
            await _context.SaveChangesAsync();
        }

        await Clients.Caller.SendAsync("ReceiveConversationData", conversation);

    }
}
