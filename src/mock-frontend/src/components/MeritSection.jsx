import React, { useState } from 'react';

const API_BASE_URL = 'https://localhost:7214/api';

const mockRecalculateMerit = {
    contentId: "00000000-0000-0000-0000-000000000001",
    contentType: "Post",
    components: {
        clarity: 0.85,
        novelty: 0.75,
        contribution: 0.80,
        civility: 0.90,
        relevance: 0.85
    }
};

export const MeritSection = ({ token }) => {
    const [userId, setUserId] = useState('');
    const [contentData, setContentData] = useState({
        contentId: '',
        contentType: 'Post'
    });
    const [timeFrame, setTimeFrame] = useState('monthly');
    const [dateRange, setDateRange] = useState({
        start: '',
        end: ''
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

    return (
        <div className="border p-4 mb-4">
            <h2 className="text-lg font-bold mb-4">Merit Score & Reputation</h2>

            {/* Get User Merit Score */}
            <div className="mb-6">
                <h3 className="font-bold mb-2">Get User Merit Score</h3>
                <input
                    type="text"
                    placeholder="User ID"
                    value={userId}
                    onChange={(e) => setUserId(e.target.value)}
                    className="border p-1 mr-2"
                />
                <button
                    onClick={() => handleApi(`/users/${userId}/merit-score`, 'GET')}
                    className="bg-blue-500 text-white px-2 py-1"
                >
                    Get Merit Score
                </button>
            </div>

            {/* Get Merit History */}
            <div className="mb-6">
                <h3 className="font-bold mb-2">Get Merit History</h3>
                <div className="grid grid-cols-2 gap-2 mb-2">
                    <input
                        type="text"
                        placeholder="User ID"
                        value={userId}
                        onChange={(e) => setUserId(e.target.value)}
                        className="border p-1"
                    />
                    <select
                        value={timeFrame}
                        onChange={(e) => setTimeFrame(e.target.value)}
                        className="border p-1"
                    >
                        <option value="daily">Daily</option>
                        <option value="weekly">Weekly</option>
                        <option value="monthly">Monthly</option>
                        <option value="yearly">Yearly</option>
                    </select>
                    <input
                        type="date"
                        placeholder="Start Date"
                        value={dateRange.start}
                        onChange={(e) => setDateRange({ ...dateRange, start: e.target.value })}
                        className="border p-1"
                    />
                    <input
                        type="date"
                        placeholder="End Date"
                        value={dateRange.end}
                        onChange={(e) => setDateRange({ ...dateRange, end: e.target.value })}
                        className="border p-1"
                    />
                </div>
                <button
                    onClick={() => handleApi(
                        `/users/${userId}/merit-history?timeFrame=${timeFrame}&start=${dateRange.start}&end=${dateRange.end}`,
                        'GET'
                    )}
                    className="bg-blue-500 text-white px-2 py-1"
                >
                    Get History
                </button>
            </div>

            {/* Recalculate Merit Score */}
            <div className="mb-6">
                <h3 className="font-bold mb-2">Recalculate Merit Score</h3>
                <div className="mb-2">
                    <button onClick={() => copyMockData(mockRecalculateMerit)} className="mr-2 bg-gray-200 px-2 py-1">
                        Copy Mock Data
                    </button>
                </div>
                <div className="grid grid-cols-2 gap-2 mb-2">
                    <input
                        type="text"
                        placeholder="Content ID"
                        value={contentData.contentId}
                        onChange={(e) => setContentData({ ...contentData, contentId: e.target.value })}
                        className="border p-1"
                    />
                    <select
                        value={contentData.contentType}
                        onChange={(e) => setContentData({ ...contentData, contentType: e.target.value })}
                        className="border p-1"
                    >
                        <option value="Post">Post</option>
                        <option value="Comment">Comment</option>
                    </select>
                </div>
                <button
                    onClick={() => handleApi('/moderation/recalculate-merit', 'POST', contentData)}
                    className="bg-yellow-500 text-white px-2 py-1"
                >
                    Recalculate Score
                </button>
            </div>

            {/* Get Reputation Snapshots */}
            <div className="mb-6">
                <h3 className="font-bold mb-2">Get Reputation Snapshots</h3>
                <div className="grid grid-cols-2 gap-2 mb-2">
                    <input
                        type="text"
                        placeholder="User ID"
                        value={userId}
                        onChange={(e) => setUserId(e.target.value)}
                        className="border p-1"
                    />
                    <select
                        value={timeFrame}
                        onChange={(e) => setTimeFrame(e.target.value)}
                        className="border p-1"
                    >
                        <option value="daily">Daily</option>
                        <option value="weekly">Weekly</option>
                        <option value="monthly">Monthly</option>
                    </select>
                </div>
                <button
                    onClick={() => handleApi(`/users/${userId}/reputation/snapshots?timeFrame=${timeFrame}`, 'GET')}
                    className="bg-blue-500 text-white px-2 py-1"
                >
                    Get Snapshots
                </button>
            </div>

            {/* Get Merit Badges */}
            <div className="mb-6">
                <h3 className="font-bold mb-2">Get Merit Badges</h3>
                <input
                    type="text"
                    placeholder="User ID"
                    value={userId}
                    onChange={(e) => setUserId(e.target.value)}
                    className="border p-1 mr-2"
                />
                <button
                    onClick={() => handleApi(`/users/${userId}/badges`, 'GET')}
                    className="bg-blue-500 text-white px-2 py-1 mr-2"
                >
                    Get All Badges
                </button>
                <button
                    onClick={() => handleApi(`/users/${userId}/badges/progress`, 'GET')}
                    className="bg-blue-500 text-white px-2 py-1"
                >
                    Get Badge Progress
                </button>
            </div>

            {/* Get Top Contributors */}
            <div className="mb-6">
                <h3 className="font-bold mb-2">Get Top Contributors</h3>
                <select
                    value={timeFrame}
                    onChange={(e) => setTimeFrame(e.target.value)}
                    className="border p-1 mr-2"
                >
                    <option value="week">This Week</option>
                    <option value="month">This Month</option>
                    <option value="year">This Year</option>
                    <option value="all">All Time</option>
                </select>
                <button
                    onClick={() => handleApi(`/users/top-contributors?timeFrame=${timeFrame}&count=10`, 'GET')}
                    className="bg-blue-500 text-white px-2 py-1"
                >
                    Get Top Contributors
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