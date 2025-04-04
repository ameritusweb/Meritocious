import React, { useState } from 'react';

const API_BASE_URL = 'http://localhost:5000/api';

const mockPost = {
    title: "Understanding AI Alignment",
    content: "AI alignment is crucial for ensuring artificial intelligence systems behave in ways that are beneficial to humanity...",
    tags: ["AI", "Ethics", "Technology"],
    authorId: "00000000-0000-0000-0000-000000000000"
};

const mockFork = {
    originalPostId: "00000000-0000-0000-0000-000000000000",
    newTitle: "Alternative View on AI Alignment",
    newAuthorId: "00000000-0000-0000-0000-000000000000"
};

export const PostsSection = ({ token }) => {
    const [postData, setPostData] = useState({
        title: '',
        content: '',
        tags: []
    });
    const [postId, setPostId] = useState('');
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

    const copyMockData = (data) => {
        navigator.clipboard.writeText(JSON.stringify(data, null, 2));
    };

    const fillMockData = (data) => {
        setPostData(data);
    };

    return (
        <div className="border p-4 mb-4">
            <h2 className="text-lg font-bold mb-4">Posts</h2>

            {/* Create Post */}
            <div className="mb-6">
                <h3 className="font-bold mb-2">Create Post</h3>
                <div className="mb-2">
                    <button onClick={() => copyMockData(mockPost)} className="mr-2 bg-gray-200 px-2 py-1">
                        Copy Mock Post
                    </button>
                    <button onClick={() => fillMockData(mockPost)} className="bg-gray-200 px-2 py-1">
                        Fill Mock Post
                    </button>
                </div>
                <input
                    type="text"
                    placeholder="Title"
                    value={postData.title}
                    onChange={(e) => setPostData({ ...postData, title: e.target.value })}
                    className="border p-1 mr-2 mb-2"
                />
                <textarea
                    placeholder="Content"
                    value={postData.content}
                    onChange={(e) => setPostData({ ...postData, content: e.target.value })}
                    className="border p-1 mr-2 mb-2 w-full"
                />
                <input
                    type="text"
                    placeholder="Tags (comma-separated)"
                    value={postData.tags.join(',')}
                    onChange={(e) => setPostData({ ...postData, tags: e.target.value.split(',') })}
                    className="border p-1 mr-2 mb-2"
                />
                <button
                    onClick={() => handleApi('/posts', 'POST', postData)}
                    className="bg-green-500 text-white px-2 py-1"
                >
                    Create Post
                </button>
            </div>

            {/* Get Post */}
            <div className="mb-6">
                <h3 className="font-bold mb-2">Get Post</h3>
                <input
                    type="text"
                    placeholder="Post ID"
                    value={postId}
                    onChange={(e) => setPostId(e.target.value)}
                    className="border p-1 mr-2"
                />
                <button
                    onClick={() => handleApi(`/posts/${postId}`, 'GET')}
                    className="bg-blue-500 text-white px-2 py-1"
                >
                    Get Post
                </button>
            </div>

            {/* Update Post */}
            <div className="mb-6">
                <h3 className="font-bold mb-2">Update Post</h3>
                <input
                    type="text"
                    placeholder="Post ID"
                    value={postId}
                    onChange={(e) => setPostId(e.target.value)}
                    className="border p-1 mr-2 mb-2"
                />
                <button
                    onClick={() => handleApi(`/posts/${postId}`, 'PUT', postData)}
                    className="bg-yellow-500 text-white px-2 py-1"
                >
                    Update Post
                </button>
            </div>

            {/* Delete Post */}
            <div className="mb-6">
                <h3 className="font-bold mb-2">Delete Post</h3>
                <input
                    type="text"
                    placeholder="Post ID"
                    value={postId}
                    onChange={(e) => setPostId(e.target.value)}
                    className="border p-1 mr-2"
                />
                <button
                    onClick={() => handleApi(`/posts/${postId}`, 'DELETE')}
                    className="bg-red-500 text-white px-2 py-1"
                >
                    Delete Post
                </button>
            </div>

            {/* Fork Post */}
            <div className="mb-6">
                <h3 className="font-bold mb-2">Fork Post</h3>
                <div className="mb-2">
                    <button onClick={() => copyMockData(mockFork)} className="mr-2 bg-gray-200 px-2 py-1">
                        Copy Mock Fork Data
                    </button>
                </div>
                <input
                    type="text"
                    placeholder="Original Post ID"
                    value={postId}
                    onChange={(e) => setPostId(e.target.value)}
                    className="border p-1 mr-2"
                />
                <button
                    onClick={() => handleApi(`/posts/${postId}/fork`, 'POST', { originalPostId: postId })}
                    className="bg-purple-500 text-white px-2 py-1"
                >
                    Fork Post
                </button>
            </div>

            {/* Get Top Posts */}
            <div className="mb-6">
                <h3 className="font-bold mb-2">Get Top Posts</h3>
                <button
                    onClick={() => handleApi('/posts?sortBy=merit&count=10', 'GET')}
                    className="bg-blue-500 text-white px-2 py-1 mr-2"
                >
                    By Merit
                </button>
                <button
                    onClick={() => handleApi('/posts?sortBy=date&count=10', 'GET')}
                    className="bg-blue-500 text-white px-2 py-1 mr-2"
                >
                    By Date
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