import React, { useState } from 'react';

const API_BASE_URL = 'http://localhost:5000/api';

const mockComment = {
    postId: "00000000-0000-0000-0000-000000000000",
    content: "This is a thoughtful response that adds to the discussion...",
    parentCommentId: null // for replies, set this to parent comment ID
};

export const CommentsSection = ({ token }) => {
    const [commentData, setCommentData] = useState({
        postId: '',
        content: '',
        parentCommentId: null
    });
    const [commentId, setCommentId] = useState('');
    const [response, setResponse] = useState(null);
    const [error, setError] = useState('');

    const headers = {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`
    };

    const handleApi = async (endpoint, method, data = null) => {
        try {
            setError('');
            setResponse(null);

            const response = await fetch(API_BASE_URL + endpoint, {
                method,
                headers,
                body: data ? JSON.stringify(data) : undefined
            });

            const responseData = await response.json();
            setResponse(responseData);

            if (!response.ok) {
                throw new Error(responseData.message || 'API call failed');
            }
        } catch (err) {
            setError(err.message);
        }
    };

    const copyMockData = () => {
        navigator.clipboard.writeText(JSON.stringify(mockComment, null, 2));
    };

    const fillMockData = () => {
        setCommentData(mockComment);
    };

    return (
        <div className="border p-4 mb-4">
            <h2 className="text-lg font-bold mb-4">Comments</h2>

            {/* Add Comment */}
            <div className="mb-6">
                <h3 className="font-bold mb-2">Add Comment</h3>
                <div className="mb-2">
                    <button onClick={copyMockData} className="mr-2 bg-gray-200 px-2 py-1">
                        Copy Mock Comment
                    </button>
                    <button onClick={fillMockData} className="bg-gray-200 px-2 py-1">
                        Fill Mock Comment
                    </button>
                </div>
                <input
                    type="text"
                    placeholder="Post ID"
                    value={commentData.postId}
                    onChange={(e) => setCommentData({ ...commentData, postId: e.target.value })}
                    className="border p-1 mr-2 mb-2"
                />
                <textarea
                    placeholder="Content"
                    value={commentData.content}
                    onChange={(e) => setCommentData({ ...commentData, content: e.target.value })}
                    className="border p-1 mr-2 mb-2 w-full"
                />
                <input
                    type="text"
                    placeholder="Parent Comment ID (optional)"
                    value={commentData.parentCommentId || ''}
                    onChange={(e) => setCommentData({ ...commentData, parentCommentId: e.target.value || null })}
                    className="border p-1 mr-2 mb-2"
                />
                <button
                    onClick={() => handleApi('/comments', 'POST', commentData)}
                    className="bg-green-500 text-white px-2 py-1"
                >
                    Add Comment
                </button>
            </div>

            {/* Get Comments for Post */}
            <div className="mb-6">
                <h3 className="font-bold mb-2">Get Comments</h3>
                <input
                    type="text"
                    placeholder="Post ID"
                    value={commentData.postId}
                    onChange={(e) => setCommentData({ ...commentData, postId: e.target.value })}
                    className="border p-1 mr-2"
                />
                <button
                    onClick={() => handleApi(`/posts/${commentData.postId}/comments?sortBy=merit`, 'GET')}
                    className="bg-blue-500 text-white px-2 py-1 mr-2"
                >
                    By Merit
                </button>
                <button
                    onClick={() => handleApi(`/posts/${commentData.postId}/comments?sortBy=date`, 'GET')}
                    className="bg-blue-500 text-white px-2 py-1 mr-2"
                >
                    By Date
                </button>
                <button
                    onClick={() => handleApi(`/posts/${commentData.postId}/comments?sortBy=thread`, 'GET')}
                    className="bg-blue-500 text-white px-2 py-1"
                >
                    Threaded
                </button>
            </div>

            {/* Update Comment */}
            <div className="mb-6">
                <h3 className="font-bold mb-2">Update Comment</h3>
                <input
                    type="text"
                    placeholder="Comment ID"
                    value={commentId}
                    onChange={(e) => setCommentId(e.target.value)}
                    className="border p-1 mr-2 mb-2"
                />
                <textarea
                    placeholder="New Content"
                    value={commentData.content}
                    onChange={(e) => setCommentData({ ...commentData, content: e.target.value })}
                    className="border p-1 mr-2 mb-2 w-full"
                />
                <button
                    onClick={() => handleApi(`/comments/${commentId}`, 'PUT', { content: commentData.content })}
                    className="bg-yellow-500 text-white px-2 py-1"
                >
                    Update Comment
                </button>
            </div>

            {/* Delete Comment */}
            <div className="mb-6">
                <h3 className="font-bold mb-2">Delete Comment</h3>
                <input
                    type="text"
                    placeholder="Comment ID"
                    value={commentId}
                    onChange={(e) => setCommentId(e.target.value)}
                    className="border p-1 mr-2"
                />
                <button
                    onClick={() => handleApi(`/comments/${commentId}`, 'DELETE')}
                    className="bg-red-500 text-white px-2 py-1"
                >
                    Delete Comment
                </button>
            </div>

            {/* Get Comment Replies */}
            <div className="mb-6">
                <h3 className="font-bold mb-2">Get Replies</h3>
                <input
                    type="text"
                    placeholder="Comment ID"
                    value={commentId}
                    onChange={(e) => setCommentId(e.target.value)}
                    className="border p-1 mr-2"
                />
                <button
                    onClick={() => handleApi(`/comments/${commentId}/replies`, 'GET')}
                    className="bg-blue-500 text-white px-2 py-1"
                >
                    Get Replies
                </button>
            </div>

            {error && (
                <div className="text-red-500 mb-4">
                    {error}
                </div>
            )}

            {response && (
                <div className="mb-4">
                    <h3 className="font-bold mb-2">Response:</h3>
                    <pre className="bg-gray-100 p-2 overflow-auto">
                        {JSON.stringify(response, null, 2)}
                    </pre>
                </div>
            )}
        </div>
    );
};