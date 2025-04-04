import React, { useState } from 'react';

const API_BASE_URL = 'http://localhost:5000/api';

const mockRemix = {
    title: "Synthesis: AI Ethics and Societal Impact",
    content: "This remix combines insights from multiple perspectives...",
    sources: [
        {
            postId: "00000000-0000-0000-0000-000000000001",
            relationship: "builds-on",
            quotes: ["Important quote from source 1"]
        },
        {
            postId: "00000000-0000-0000-0000-000000000002",
            relationship: "contrasts-with",
            quotes: ["Contrasting perspective from source 2"]
        }
    ]
};

export const RemixSection = ({ token }) => {
    const [remixData, setRemixData] = useState({
        title: '',
        content: '',
        sources: []
    });
    const [remixId, setRemixId] = useState('');
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
        navigator.clipboard.writeText(JSON.stringify(mockRemix, null, 2));
    };

    const fillMockData = () => {
        setRemixData(mockRemix);
    };

    return (
        <div className="border p-4 mb-4">
            <h2 className="text-lg font-bold mb-4">Remixes</h2>

            {/* Create Remix */}
            <div className="mb-6">
                <h3 className="font-bold mb-2">Create Remix</h3>
                <div className="mb-2">
                    <button onClick={copyMockData} className="mr-2 bg-gray-200 px-2 py-1">
                        Copy Mock Remix
                    </button>
                    <button onClick={fillMockData} className="bg-gray-200 px-2 py-1">
                        Fill Mock Remix
                    </button>
                </div>
                <input
                    type="text"
                    placeholder="Title"
                    value={remixData.title}
                    onChange={(e) => setRemixData({ ...remixData, title: e.target.value })}
                    className="border p-1 mr-2 mb-2"
                />
                <textarea
                    placeholder="Content"
                    value={remixData.content}
                    onChange={(e) => setRemixData({ ...remixData, content: e.target.value })}
                    className="border p-1 mr-2 mb-2 w-full"
                />
                <textarea
                    placeholder="Sources (JSON array)"
                    value={JSON.stringify(remixData.sources, null, 2)}
                    onChange={(e) => {
                        try {
                            const sources = JSON.parse(e.target.value);
                            setRemixData({ ...remixData, sources });
                        } catch (err) {
                            // Invalid JSON - ignore
                            console.error(err);
                        }
                    }}
                    className="border p-1 mr-2 mb-2 w-full font-mono"
                />
                <button
                    onClick={() => handleApi('/remix', 'POST', remixData)}
                    className="bg-green-500 text-white px-2 py-1"
                >
                    Create Remix
                </button>
            </div>

            {/* Get Remix */}
            <div className="mb-6">
                <h3 className="font-bold mb-2">Get Remix</h3>
                <input
                    type="text"
                    placeholder="Remix ID"
                    value={remixId}
                    onChange={(e) => setRemixId(e.target.value)}
                    className="border p-1 mr-2"
                />
                <button
                    onClick={() => handleApi(`/remix/${remixId}`, 'GET')}
                    className="bg-blue-500 text-white px-2 py-1"
                >
                    Get Remix
                </button>
            </div>

            {/* Update Remix */}
            <div className="mb-6">
                <h3 className="font-bold mb-2">Update Remix</h3>
                <input
                    type="text"
                    placeholder="Remix ID"
                    value={remixId}
                    onChange={(e) => setRemixId(e.target.value)}
                    className="border p-1 mr-2 mb-2"
                />
                <button
                    onClick={() => handleApi(`/remix/${remixId}`, 'PUT', remixData)}
                    className="bg-yellow-500 text-white px-2 py-1"
                >
                    Update Remix
                </button>
            </div>

            {/* Delete Remix */}
            <div className="mb-6">
                <h3 className="font-bold mb-2">Delete Remix</h3>
                <input
                    type="text"
                    placeholder="Remix ID"
                    value={remixId}
                    onChange={(e) => setRemixId(e.target.value)}
                    className="border p-1 mr-2"
                />
                <button
                    onClick={() => handleApi(`/remix/${remixId}`, 'DELETE')}
                    className="bg-red-500 text-white px-2 py-1"
                >
                    Delete Remix
                </button>
            </div>

            {/* Get Related Remixes */}
            <div className="mb-6">
                <h3 className="font-bold mb-2">Get Related Remixes</h3>
                <input
                    type="text"
                    placeholder="Remix ID"
                    value={remixId}
                    onChange={(e) => setRemixId(e.target.value)}
                    className="border p-1 mr-2"
                />
                <button
                    onClick={() => handleApi(`/remix/${remixId}/related`, 'GET')}
                    className="bg-blue-500 text-white px-2 py-1"
                >
                    Get Related
                </button>
            </div>

            {/* Get Trending Remixes */}
            <div className="mb-6">
                <h3 className="font-bold mb-2">Get Trending Remixes</h3>
                <button
                    onClick={() => handleApi('/remix/trending', 'GET')}
                    className="bg-blue-500 text-white px-2 py-1"
                >
                    Get Trending
                </button>
            </div>

            {/* Generate Insights */}
            <div className="mb-6">
                <h3 className="font-bold mb-2">Generate AI Insights</h3>
                <input
                    type="text"
                    placeholder="Remix ID"
                    value={remixId}
                    onChange={(e) => setRemixId(e.target.value)}
                    className="border p-1 mr-2"
                />
                <button
                    onClick={() => handleApi(`/remix/${remixId}/insights`, 'POST')}
                    className="bg-purple-500 text-white px-2 py-1"
                >
                    Generate Insights
                </button>
            </div>

            {/* Get Synthesis Score */}
            <div className="mb-6">
                <h3 className="font-bold mb-2">Get Synthesis Score</h3>
                <input
                    type="text"
                    placeholder="Remix ID"
                    value={remixId}
                    onChange={(e) => setRemixId(e.target.value)}
                    className="border p-1 mr-2"
                />
                <button
                    onClick={() => handleApi(`/remix/${remixId}/score`, 'GET')}
                    className="bg-blue-500 text-white px-2 py-1"
                >
                    Get Score
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