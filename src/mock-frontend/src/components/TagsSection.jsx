import React, { useState } from 'react';

const API_BASE_URL = 'http://localhost:5000/api';

const mockTag = {
    name: "AI Ethics",
    description: "Discussions about ethical considerations in artificial intelligence development",
    relatedTags: ["Technology", "Ethics", "AI Safety"]
};

const mockTagRelationship = {
    parentTagId: "00000000-0000-0000-0000-000000000001",
    childTagId: "00000000-0000-0000-0000-000000000002",
    relationType: "subtopic"
};

export const TagsSection = ({ token }) => {
    const [tagData, setTagData] = useState({
        name: '',
        description: '',
        relatedTags: []
    });
    const [tagName, setTagName] = useState('');
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

    const fillMockData = () => {
        setTagData(mockTag);
    };

    return (
        <div className="border p-4 mb-4">
            <h2 className="text-lg font-bold mb-4">Tags</h2>

            {/* Create Tag */}
            <div className="mb-6">
                <h3 className="font-bold mb-2">Create Tag</h3>
                <div className="mb-2">
                    <button onClick={() => copyMockData(mockTag)} className="mr-2 bg-gray-200 px-2 py-1">
                        Copy Mock Tag
                    </button>
                    <button onClick={fillMockData} className="bg-gray-200 px-2 py-1">
                        Fill Mock Tag
                    </button>
                </div>
                <input
                    type="text"
                    placeholder="Name"
                    value={tagData.name}
                    onChange={(e) => setTagData({ ...tagData, name: e.target.value })}
                    className="border p-1 mr-2 mb-2"
                />
                <textarea
                    placeholder="Description"
                    value={tagData.description}
                    onChange={(e) => setTagData({ ...tagData, description: e.target.value })}
                    className="border p-1 mr-2 mb-2 w-full"
                />
                <input
                    type="text"
                    placeholder="Related Tags (comma-separated)"
                    value={tagData.relatedTags.join(',')}
                    onChange={(e) => setTagData({ ...tagData, relatedTags: e.target.value.split(',') })}
                    className="border p-1 mr-2 mb-2"
                />
                <button
                    onClick={() => handleApi('/tags', 'POST', tagData)}
                    className="bg-green-500 text-white px-2 py-1"
                >
                    Create Tag
                </button>
            </div>

            {/* Get Tag */}
            <div className="mb-6">
                <h3 className="font-bold mb-2">Get Tag</h3>
                <input
                    type="text"
                    placeholder="Tag Name"
                    value={tagName}
                    onChange={(e) => setTagName(e.target.value)}
                    className="border p-1 mr-2"
                />
                <button
                    onClick={() => handleApi(`/tags/${tagName}`, 'GET')}
                    className="bg-blue-500 text-white px-2 py-1"
                >
                    Get Tag
                </button>
            </div>

            {/* Get Popular Tags */}
            <div className="mb-6">
                <h3 className="font-bold mb-2">Get Popular Tags</h3>
                <button
                    onClick={() => handleApi('/tags/popular', 'GET')}
                    className="bg-blue-500 text-white px-2 py-1"
                >
                    Get Popular
                </button>
            </div>

            {/* Search Tags */}
            <div className="mb-6">
                <h3 className="font-bold mb-2">Search Tags</h3>
                <input
                    type="text"
                    placeholder="Search Term"
                    value={tagName}
                    onChange={(e) => setTagName(e.target.value)}
                    className="border p-1 mr-2"
                />
                <button
                    onClick={() => handleApi(`/tags/search?searchTerm=${tagName}`, 'GET')}
                    className="bg-blue-500 text-white px-2 py-1"
                >
                    Search
                </button>
            </div>

            {/* Add Tag Relationship */}
            <div className="mb-6">
                <h3 className="font-bold mb-2">Add Tag Relationship</h3>
                <div className="mb-2">
                    <button onClick={() => copyMockData(mockTagRelationship)} className="mr-2 bg-gray-200 px-2 py-1">
                        Copy Mock Relationship
                    </button>
                </div>
                <button
                    onClick={() => handleApi('/tags/relationships', 'POST', mockTagRelationship)}
                    className="bg-green-500 text-white px-2 py-1"
                >
                    Add Relationship
                </button>
            </div>

            {/* Follow Tag */}
            <div className="mb-6">
                <h3 className="font-bold mb-2">Follow/Unfollow Tag</h3>
                <input
                    type="text"
                    placeholder="Tag Name"
                    value={tagName}
                    onChange={(e) => setTagName(e.target.value)}
                    className="border p-1 mr-2"
                />
                <button
                    onClick={() => handleApi(`/tags/${tagName}/follow`, 'POST')}
                    className="bg-green-500 text-white px-2 py-1 mr-2"
                >
                    Follow
                </button>
                <button
                    onClick={() => handleApi(`/tags/${tagName}/follow`, 'DELETE')}
                    className="bg-red-500 text-white px-2 py-1"
                >
                    Unfollow
                </button>
            </div>

            {/* Get Following Tags */}
            <div className="mb-6">
                <h3 className="font-bold mb-2">Get Following Tags</h3>
                <button
                    onClick={() => handleApi('/tags/following', 'GET')}
                    className="bg-blue-500 text-white px-2 py-1"
                >
                    Get Following
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