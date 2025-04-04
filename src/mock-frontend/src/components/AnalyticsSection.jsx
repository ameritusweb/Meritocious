import React, { useState } from 'react';

const API_BASE_URL = 'http://localhost:5000/api';

const mockAuditLogQuery = {
    startDate: "2024-01-01",
    endDate: "2024-12-31",
    includeAdminActions: true,
    includeSecurityEvents: true,
    includeLoginAttempts: true,
    includeApiUsage: true
};

export const AnalyticsSection = ({ token }) => {
    const [dateRange, setDateRange] = useState({
        startDate: '',
        endDate: ''
    });
    const [contentId, setContentId] = useState('');
    const [contentType, setContentType] = useState('Post');
    const [userId, setUserId] = useState('');
    const [tagName, setTagName] = useState('');
    const [endpointPath, setEndpointPath] = useState('');
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
            <h2 className="text-lg font-bold mb-4">Analytics & Monitoring</h2>

            {/* Content Analytics */}
            <div className="mb-6">
                <h3 className="font-bold mb-2">Content Analytics</h3>
                <div className="grid grid-cols-2 gap-2 mb-2">
                    <input
                        type="text"
                        placeholder="Content ID"
                        value={contentId}
                        onChange={(e) => setContentId(e.target.value)}
                        className="border p-1"
                    />
                    <select
                        value={contentType}
                        onChange={(e) => setContentType(e.target.value)}
                        className="border p-1"
                    >
                        <option value="Post">Post</option>
                        <option value="Comment">Comment</option>
                    </select>
                </div>
                <button
                    onClick={() => handleApi(`/analytics/content/${contentId}?type=${contentType}`, 'GET')}
                    className="bg-blue-500 text-white px-2 py-1 mr-2"
                >
                    Get Analytics
                </button>
                <button
                    onClick={() => handleApi(`/analytics/content/${contentId}/engagement?type=${contentType}`, 'GET')}
                    className="bg-blue-500 text-white px-2 py-1"
                >
                    Get Engagement
                </button>
            </div>

            {/* User Analytics */}
            <div className="mb-6">
                <h3 className="font-bold mb-2">User Analytics</h3>
                <div className="grid grid-cols-2 gap-2 mb-2">
                    <input
                        type="text"
                        placeholder="User ID"
                        value={userId}
                        onChange={(e) => setUserId(e.target.value)}
                        className="border p-1"
                    />
                    <div className="flex items-center">
                        <input
                            type="date"
                            value={dateRange.startDate}
                            onChange={(e) => setDateRange({ ...dateRange, startDate: e.target.value })}
                            className="border p-1 w-1/2"
                        />
                        <span className="mx-2">to</span>
                        <input
                            type="date"
                            value={dateRange.endDate}
                            onChange={(e) => setDateRange({ ...dateRange, endDate: e.target.value })}
                            className="border p-1 w-1/2"
                        />
                    </div>
                </div>
                <button
                    onClick={() => handleApi(
                        `/analytics/users/${userId}/activity?startDate=${dateRange.startDate}&endDate=${dateRange.endDate}`,
                        'GET'
                    )}
                    className="bg-blue-500 text-white px-2 py-1 mr-2"
                >
                    Get Activity
                </button>
                <button
                    onClick={() => handleApi(`/analytics/users/${userId}/engagement`, 'GET')}
                    className="bg-blue-500 text-white px-2 py-1"
                >
                    Get Engagement
                </button>
            </div>

            {/* Tag Analytics */}
            <div className="mb-6">
                <h3 className="font-bold mb-2">Tag Analytics</h3>
                <div className="grid grid-cols-2 gap-2 mb-2">
                    <input
                        type="text"
                        placeholder="Tag Name"
                        value={tagName}
                        onChange={(e) => setTagName(e.target.value)}
                        className="border p-1"
                    />
                    <select
                        value={dateRange.startDate}
                        onChange={(e) => setDateRange({ ...dateRange, startDate: e.target.value })}
                        className="border p-1"
                    >
                        <option value="day">Last Day</option>
                        <option value="week">Last Week</option>
                        <option value="month">Last Month</option>
                        <option value="year">Last Year</option>
                    </select>
                </div>
                <button
                    onClick={() => handleApi(`/analytics/tags/${tagName}/stats`, 'GET')}
                    className="bg-blue-500 text-white px-2 py-1 mr-2"
                >
                    Get Stats
                </button>
                <button
                    onClick={() => handleApi(`/analytics/tags/${tagName}/trends`, 'GET')}
                    className="bg-blue-500 text-white px-2 py-1"
                >
                    Get Trends
                </button>
            </div>

            {/* API Usage Analytics */}
            <div className="mb-6">
                <h3 className="font-bold mb-2">API Usage Analytics</h3>
                <div className="grid grid-cols-2 gap-2 mb-2">
                    <input
                        type="text"
                        placeholder="Endpoint Path"
                        value={endpointPath}
                        onChange={(e) => setEndpointPath(e.target.value)}
                        className="border p-1"
                    />
                    <select
                        value={dateRange.startDate}
                        onChange={(e) => setDateRange({ ...dateRange, startDate: e.target.value })}
                        className="border p-1"
                    >
                        <option value="hour">Last Hour</option>
                        <option value="day">Last Day</option>
                        <option value="week">Last Week</option>
                    </select>
                </div>
                <button
                    onClick={() => handleApi(
                        `/analytics/api/usage?endpoint=${endpointPath}&timeFrame=${dateRange.startDate}`,
                        'GET'
                    )}
                    className="bg-blue-500 text-white px-2 py-1"
                >
                    Get API Usage
                </button>
            </div>

            {/* System Logs */}
            <div className="mb-6">
                <h3 className="font-bold mb-2">System Logs</h3>
                <div className="mb-2">
                    <button onClick={() => copyMockData(mockAuditLogQuery)} className="mr-2 bg-gray-200 px-2 py-1">
                        Copy Mock Query
                    </button>
                </div>
                <div className="grid grid-cols-2 gap-2 mb-2">
                    <input
                        type="date"
                        value={dateRange.startDate}
                        onChange={(e) => setDateRange({ ...dateRange, startDate: e.target.value })}
                        className="border p-1"
                    />
                    <input
                        type="date"
                        value={dateRange.endDate}
                        onChange={(e) => setDateRange({ ...dateRange, endDate: e.target.value })}
                        className="border p-1"
                    />
                </div>
                <div className="mb-2">
                    <label className="mr-4">
                        <input
                            type="checkbox"
                            checked={true}
                            className="mr-1"
                        />
                        Admin Actions
                    </label>
                    <label className="mr-4">
                        <input
                            type="checkbox"
                            checked={true}
                            className="mr-1"
                        />
                        Security Events
                    </label>
                    <label className="mr-4">
                        <input
                            type="checkbox"
                            checked={true}
                            className="mr-1"
                        />
                        Login Attempts
                    </label>
                    <label>
                        <input
                            type="checkbox"
                            checked={true}
                            className="mr-1"
                        />
                        API Usage
                    </label>
                </div>
                <button
                    onClick={() => handleApi('/analytics/logs', 'GET')}
                    className="bg-blue-500 text-white px-2 py-1 mr-2"
                >
                    View Logs
                </button>
                <button
                    onClick={() => handleApi('/analytics/logs/export', 'POST', {
                        startDate: dateRange.startDate,
                        endDate: dateRange.endDate,
                        includeAll: true
                    })}
                    className="bg-green-500 text-white px-2 py-1"
                >
                    Export Logs
                </button>
            </div>

            {/* Security Overview */}
            <div className="mb-6">
                <h3 className="font-bold mb-2">Security Overview</h3>
                <button
                    onClick={() => handleApi('/security/overview', 'GET')}
                    className="bg-blue-500 text-white px-2 py-1"
                >
                    Get Overview
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