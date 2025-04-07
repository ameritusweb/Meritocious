import React, { useState } from 'react';

const API_BASE_URL = 'https://localhost:7214/api';

const mockSearchFilters = {
    contentTypes: ["Post", "Comment"],
    minMeritScore: 0.7,
    authorId: null,
    tags: ["AI", "Ethics"],
    dateRange: {
        start: "2024-01-01",
        end: "2024-12-31"
    }
};

export const DiscoverySection = ({ token }) => {
    const [searchData, setSearchData] = useState({
        searchTerm: '',
        contentTypes: ['Post', 'Comment'],
        sortBy: 'relevance',
        page: 1,
        pageSize: 10,
        minMeritScore: 0,
        tags: []
    });
    const [topicName, setTopicName] = useState('');
    const [timeFrame, setTimeFrame] = useState('week');
    const [contentId, setContentId] = useState('');
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

    return (
        <div className="border p-4 mb-4">
            <h2 className="text-lg font-bold mb-4">Search & Discovery</h2>

            {/* Search Content */}
            <div className="mb-6">
                <h3 className="font-bold mb-2">Search Content</h3>
                <div className="mb-2">
                    <button onClick={() => copyMockData(mockSearchFilters)} className="mr-2 bg-gray-200 px-2 py-1">
                        Copy Mock Filters
                    </button>
                </div>
                <input
                    type="text"
                    placeholder="Search Term"
                    value={searchData.searchTerm}
                    onChange={(e) => setSearchData({ ...searchData, searchTerm: e.target.value })}
                    className="border p-1 w-full mb-2"
                />
                <div className="grid grid-cols-2 gap-2 mb-2">
                    <select
                        multiple
                        value={searchData.contentTypes}
                        onChange={(e) => setSearchData({
                            ...searchData,
                            contentTypes: Array.from(e.target.selectedOptions, option => option.value)
                        })}
                        className="border p-1"
                    >
                        <option value="Post">Posts</option>
                        <option value="Comment">Comments</option>
                    </select>
                    <select
                        value={searchData.sortBy}
                        onChange={(e) => setSearchData({ ...searchData, sortBy: e.target.value })}
                        className="border p-1"
                    >
                        <option value="relevance">Relevance</option>
                        <option value="merit">Merit Score</option>
                        <option value="date">Date</option>
                    </select>
                    <input
                        type="number"
                        placeholder="Min Merit Score (0-1)"
                        value={searchData.minMeritScore}
                        onChange={(e) => setSearchData({ ...searchData, minMeritScore: parseFloat(e.target.value) })}
                        className="border p-1"
                        min="0"
                        max="1"
                        step="0.1"
                    />
                    <input
                        type="text"
                        placeholder="Tags (comma-separated)"
                        value={searchData.tags.join(',')}
                        onChange={(e) => setSearchData({ ...searchData, tags: e.target.value.split(',') })}
                        className="border p-1"
                    />
                </div>
                <button
                    onClick={() => {
                        const params = new URLSearchParams({
                            term: searchData.searchTerm,
                            contentTypes: searchData.contentTypes.join(','),
                            sortBy: searchData.sortBy,
                            minMeritScore: searchData.minMeritScore,
                            tags: searchData.tags.join(','),
                            page: searchData.page,
                            pageSize: searchData.pageSize
                        });
                        handleApi(`/search/content?${params}`, 'GET');
                    }}
                    className="bg-blue-500 text-white px-2 py-1"
                >
                    Search
                </button>
            </div>

            {/* Get Trending Topics */}
            <div className="mb-6">
                <h3 className="font-bold mb-2">Get Trending Topics</h3>
                <select
                    value={timeFrame}
                    onChange={(e) => setTimeFrame(e.target.value)}
                    className="border p-1 mr-2"
                >
                    <option value="hour">Last Hour</option>
                    <option value="day">Last Day</option>
                    <option value="week">Last Week</option>
                    <option value="month">Last Month</option>
                </select>
                <button
                    onClick={() => handleApi(`/tags/trending?timeFrame=${timeFrame}`, 'GET')}
                    className="bg-blue-500 text-white px-2 py-1"
                >
                    Get Trending Topics
                </button>
            </div>

            {/* Get Posts by Topic */}
            <div className="mb-6">
                <h3 className="font-bold mb-2">Get Posts by Topic</h3>
                <input
                    type="text"
                    placeholder="Topic Name"
                    value={topicName}
                    onChange={(e) => setTopicName(e.target.value)}
                    className="border p-1 mr-2"
                />
                <button
                    onClick={() => handleApi(`/tags/${topicName}/posts`, 'GET')}
                    className="bg-blue-500 text-white px-2 py-1"
                >
                    Get Posts
                </button>
            </div>

            {/* Get Similar Content */}
            <div className="mb-6">
                <h3 className="font-bold mb-2">Get Similar Content</h3>
                <input
                    type="text"
                    placeholder="Content ID"
                    value={contentId}
                    onChange={(e) => setContentId(e.target.value)}
                    className="border p-1 mr-2"
                />
                <button
                    onClick={() => handleApi(`/posts/${contentId}/similar`, 'GET')}
                    className="bg-blue-500 text-white px-2 py-1"
                >
                    Get Similar
                </button>
            </div>

            {/* Get Recommended Posts */}
            <div className="mb-6">
                <h3 className="font-bold mb-2">Get Recommended Posts</h3>
                <div className="grid grid-cols-2 gap-2 mb-2">
                    <input
                        type="number"
                        placeholder="Count"
                        value={searchData.pageSize}
                        onChange={(e) => setSearchData({ ...searchData, pageSize: parseInt(e.target.value) })}
                        className="border p-1"
                    />
                    <input
                        type="text"
                        placeholder="Excluded Post IDs (comma-separated)"
                        className="border p-1"
                    />
                </div>
                <button
                    onClick={() => handleApi('/posts/recommended', 'GET')}
                    className="bg-blue-500 text-white px-2 py-1"
                >
                    Get Recommendations
                </button>
            </div>

            {/* Get Topic Clusters */}
            <div className="mb-6">
                <h3 className="font-bold mb-2">Get Topic Clusters</h3>
                <select
                    value={timeFrame}
                    onChange={(e) => setTimeFrame(e.target.value)}
                    className="border p-1 mr-2"
                >
                    <option value="day">Last Day</option>
                    <option value="week">Last Week</option>
                    <option value="month">Last Month</option>
                </select>
                <button
                    onClick={() => handleApi(`/topics/clusters?timeFrame=${timeFrame}`, 'GET')}
                    className="bg-blue-500 text-white px-2 py-1"
                >
                    Get Clusters
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