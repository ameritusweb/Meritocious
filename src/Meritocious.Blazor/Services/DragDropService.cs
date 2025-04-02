using System.Text.Json;
using Meritocious.Common.DTOs.Content;

namespace Meritocious.Blazor.Services;

public class DragDropService
{
    public const string PostDragType = "application/x-meritocious-post";
    private PostDto _draggedPost;
    
    public event Action<PostDto> OnPostDragged;
    public event Action<PostDto> OnPostDropped;
    public event Action OnDragEnd;

    public void StartPostDrag(PostDto post)
    {
        _draggedPost = post;
        OnPostDragged?.Invoke(post);
    }

    public PostDto GetDraggedPost()
    {
        return _draggedPost;
    }

    public void HandleDrop()
    {
        if (_draggedPost != null)
        {
            OnPostDropped?.Invoke(_draggedPost);
            _draggedPost = null;
        }
    }

    public void EndDrag()
    {
        _draggedPost = null;
        OnDragEnd?.Invoke();
    }

    public string SerializePost(PostDto post)
    {
        return JsonSerializer.Serialize(post);
    }

    public PostDto DeserializePost(string data)
    {
        try
        {
            return JsonSerializer.Deserialize<PostDto>(data);
        }
        catch
        {
            return null;
        }
    }
}