import React, { useState } from 'react';

const API_BASE_URL = 'http://localhost:5000/api';

const mockSubstack = {
    name: "AI Insights Weekly",
    subdomain: "ai-insights",
    customDomain: "aiinsights.com",
    authorName: "Dr. AI Researcher",
    description: "Weekly analysis of AI developments and their implications",
    logoUrl: "https://example.com/logo.png",
    coverImageUrl: "https://example.com/cover.png",
    twitterHandle: "@aiinsights"
};

const mockImport = {
    postUrl: "https://aiinsights.substack.com/p/future-of-ai",
    substackName: "ai-insights",
    importAsRemix: true,
    remixNotes: "Importing for discussion and analysis"
};

export const SubstackSection = ({ token }) => {
    const [substackData, setSubstackData] = useState({
        name: '',
        subdomain: '',
        customDomain: '',
        authorName: '',
        description: '',
        logoUrl: '',
        coverImageUrl: '',
        twitterHandle: ''
    });
    const [substackId, setSubstackId] = useState('');
    const [importData, setImportData] = useState({
        postUrl: '',
        substackName: '',
        importAsRemix: false,
        remixNotes: ''
    });
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

    const fillMockData = (target) => {
        if (target === 'substack') {
            setSubstackData(mockSubstack);
        } else if (target === 'import') {
            setImportData(mockImport);
        }
    };

    return (
        <div className="border p-4 mb-4">
            <h2 className="text-lg font-bold mb-4">Substacks</h2>

            {/* Create Substack */}
            <div className="mb-6">
                <h3 className="font-bold mb-2">Create Substack</h3>
                <div className="mb-2">
                    <button onClick={() => copyMockData(mockSubstack)} className="mr-2 bg-gray-200 px-2 py-1">
                        Copy Mock Substack
                    </button>
                    <button onClick={() => fillMockData('substack')} className="bg-gray-200 px-2 py-1">
                        Fill Mock Substack
                    </button>
                </div>
                <div className="grid grid-cols-2 gap-2 mb-2">
                    <input
                        type="text"
                        placeholder="Name"
                        value={substackData.name}
                        onChange={(e) => setSubstackData({ ...substackData, name: e.target.value })}
                        className="border p-1"
                    />
                    <input
                        type="text"
                        placeholder="Subdomain"
                        value={substackData.subdomain}
                        onChange={(e) => setSubstackData({ ...substackData, subdomain: e.target.value })}
                        className="border p-1"
                    />
                    <input
                        type="text"
                        placeholder="Custom Domain"
                        value={substackData.customDomain}
                        onChange={(e) => setSubstackData({ ...substackData, customDomain: e.target.value })}
                        className="border p-1"
                    />
                    <input
                        type="text"
                        placeholder="Author Name"
                        value={substackData.authorName}
                        onChange={(e) => setSubstackData({ ...substackData, authorName: e.target.value })}
                        className="border p-1"
                    />
                </div>
                <textarea
                    placeholder="Description"
                    value={substackData.description}
                    onChange={(e) => setSubstackData({ ...substackData, description: e.target.value })}
                    className="border p-1 w-full mb-2"
                />
                <div className="grid grid-cols-2 gap-2 mb-2">
                    <input
                        type="text"
                        placeholder="Logo URL"
                        value={substackData.logoUrl}
                        onChange={(e) => setSubstackData({ ...substackData, logoUrl: e.target.value })}
                        className="border p-1"
                    />
                    <input
                        type="text"
                        placeholder="Cover Image URL"
                        value={substackData.coverImageUrl}
                        onChange={(e) => setSubstackData({ ...substackData, coverImageUrl: e.target.value })}
                        className="border p-1"
                    />
                    <input
                        type="text"
                        placeholder="Twitter Handle"
                        value={substackData.twitterHandle}
                        onChange={(e) => setSubstackData({ ...substackData, twitterHandle: e.target.value })}
                        className="border p-1"
                    />
                </div>
                <button
                    onClick={() => handleApi('/substack', 'POST', substackData)}
                    className="bg-green-500 text-white px-2 py-1"
                >
                    Create Substack
                </button>
            </div>

            {/* Import Post */}
            <div className="mb-6">
                <h3 className="font-bold mb-2">Import Post</h3>
                <div className="mb-2">
                    <button onClick={() => copyMockData(mockImport)} className="mr-2 bg-gray-200 px-2 py-1">
                        Copy Mock Import
                    </button>
                    <button onClick={() => fillMockData('import')} className="bg-gray-200 px-2 py-1">
                        Fill Mock Import
                    </button>
                </div>
                <input
                    type="text"
                    placeholder="Post URL"
                    value={importData.postUrl}
                    onChange={(e) => setImportData({ ...importData, postUrl: e.target.value })}
                    className="border p-1 w-full mb-2"
                />
                <input
                    type="text"
                    placeholder="Substack Name"
                    value={importData.substackName}
                    onChange={(e) => setImportData({ ...importData, substackName: e.target.value })}
                    className="border p-1 mb-2 w-full"
                />
                <div className="mb-2">
                    <label className="mr-2">
                        <input
                            type="checkbox"
                            checked={importData.importAsRemix}
                            onChange={(e) => setImportData({ ...importData, importAsRemix: e.target.checked })}
                            className="mr-1"
                        />
                        Import as Remix
                    </label>
                </div>
                <textarea
                    placeholder="Remix Notes"
                    value={importData.remixNotes}
                    onChange={(e) => setImportData({ ...importData, remixNotes: e.target.value })}
                    className="border p-1 w-full mb-2"
                />
                <button
                    onClick={() => handleApi('/substack/import', 'POST', importData)}
                    className="bg-green-500 text-white px-2 py-1"
                >
                    Import Post
                </button>
            </div>

            {/* Get Substack */}
            <div className="mb-6">
                <h3 className="font-bold mb-2">Get Substack</h3>
                <input
                    type="text"
                    placeholder="Substack ID"
                    value={substackId}
                    onChange={(e) => setSubstackId(e.target.value)}
                    className="border p-1 mr-2"
                />
                <button
                    onClick={() => handleApi(`/substack/${substackId}`, 'GET')}
                    className="bg-blue-500 text-white px-2 py-1"
                >
                    Get Substack
                </button>
            </div>

            {/* Get Trending/Recommended */}
            <div className="mb-6">
                <h3 className="font-bold mb-2">Get Trending/Recommended</h3>
                <button
                    onClick={() => handleApi('/substack/trending', 'GET')}
                    className="bg-blue-500 text-white px-2 py-1 mr-2"
                >
                    Get Trending
                </button>
                <button
                    onClick={() => handleApi('/substack/recommended', 'GET')}
                    className="bg-blue-500 text-white px-2 py-1"
                >
                    Get Recommended
                </button>
            </div>

            {/* Follow/Unfollow */}
            <div className="mb-6">
                <h3 className="font-bold mb-2">Follow/Unfollow</h3>
                <input
                    type="text"
                    placeholder="Substack ID"
                    value={substackId}
                    onChange={(e) => setSubstackId(e.target.value)}
                    className="border p-1 mr-2"
                />
                <button
                    onClick={() => handleApi(`/substack/${substackId}/follow`, 'POST')}
                    className="bg-green-500 text-white px-2 py-1 mr-2"
                >
                    Follow
                </button>
                <button
                    onClick={() => handleApi(`/substack/${substackId}/follow`, 'DELETE')}
                    className="bg-red-500 text-white px-2 py-1"
                >
                    Unfollow
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