import React, { useState } from 'react';

const API_BASE_URL = 'http://localhost:5000/api';

const mockReport = {
    contentId: "00000000-0000-0000-0000-000000000001",
    contentType: "Post", // "Post" or "Comment"
    reportType: "inappropriate",
    description: "This content contains misleading information",
    reporterId: "00000000-0000-0000-0000-000000000002"
};

const mockModeration = {
    contentId: "00000000-0000-0000-0000-000000000001",
    contentType: "Post",
    action: {
        actionType: "Warning",
        reason: "Content needs revision for clarity",
        duration: "24h"
    },
    moderatorNotes: "First warning - unclear content"
};

const mockResolveReport = {
    resolution: "warned",
    notes: "Issued warning to user",
    action: {
        actionType: "Warning",
        duration: "24h"
    }
};

export const ModerationSection = ({ token }) => {
    const [reportData, setReportData] = useState({
        contentId: '',
        contentType: 'Post',
        reportType: '',
        description: '',
        reporterId: ''
    });
    const [moderationData, setModerationData] = useState({
        contentId: '',
        contentType: 'Post',
        action: {
            actionType: '',
            reason: '',
            duration: ''
        },
        moderatorNotes: ''
    });
    const [reportId, setReportId] = useState('');
    const [resolveData, setResolveData] = useState({
        resolution: '',
        notes: '',
        action: {
            actionType: '',
            duration: ''
        }
    });
    const [timeFrame, setTimeFrame] = useState('day');
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
            <h2 className="text-lg font-bold mb-4">Moderation</h2>

            {/* Report Content */}
            <div className="mb-6">
                <h3 className="font-bold mb-2">Report Content</h3>
                <div className="mb-2">
                    <button onClick={() => copyMockData(mockReport)} className="mr-2 bg-gray-200 px-2 py-1">
                        Copy Mock Report
                    </button>
                </div>
                <div className="grid grid-cols-2 gap-2 mb-2">
                    <input
                        type="text"
                        placeholder="Content ID"
                        value={reportData.contentId}
                        onChange={(e) => setReportData({ ...reportData, contentId: e.target.value })}
                        className="border p-1"
                    />
                    <select
                        value={reportData.contentType}
                        onChange={(e) => setReportData({ ...reportData, contentType: e.target.value })}
                        className="border p-1"
                    >
                        <option value="Post">Post</option>
                        <option value="Comment">Comment</option>
                    </select>
                    <input
                        type="text"
                        placeholder="Report Type"
                        value={reportData.reportType}
                        onChange={(e) => setReportData({ ...reportData, reportType: e.target.value })}
                        className="border p-1"
                    />
                    <input
                        type="text"
                        placeholder="Reporter ID"
                        value={reportData.reporterId}
                        onChange={(e) => setReportData({ ...reportData, reporterId: e.target.value })}
                        className="border p-1"
                    />
                </div>
                <textarea
                    placeholder="Description"
                    value={reportData.description}
                    onChange={(e) => setReportData({ ...reportData, description: e.target.value })}
                    className="border p-1 w-full mb-2"
                />
                <button
                    onClick={() => handleApi('/moderation/reports', 'POST', reportData)}
                    className="bg-red-500 text-white px-2 py-1"
                >
                    Submit Report
                </button>
            </div>

            {/* Moderate Content */}
            <div className="mb-6">
                <h3 className="font-bold mb-2">Moderate Content</h3>
                <div className="mb-2">
                    <button onClick={() => copyMockData(mockModeration)} className="mr-2 bg-gray-200 px-2 py-1">
                        Copy Mock Moderation
                    </button>
                </div>
                <div className="grid grid-cols-2 gap-2 mb-2">
                    <input
                        type="text"
                        placeholder="Content ID"
                        value={moderationData.contentId}
                        onChange={(e) => setModerationData({ ...moderationData, contentId: e.target.value })}
                        className="border p-1"
                    />
                    <select
                        value={moderationData.contentType}
                        onChange={(e) => setModerationData({ ...moderationData, contentType: e.target.value })}
                        className="border p-1"
                    >
                        <option value="Post">Post</option>
                        <option value="Comment">Comment</option>
                    </select>
                    <select
                        value={moderationData.action.actionType}
                        onChange={(e) => setModerationData({
                            ...moderationData,
                            action: { ...moderationData.action, actionType: e.target.value }
                        })}
                        className="border p-1"
                    >
                        <option value="">Select Action</option>
                        <option value="Warning">Warning</option>
                        <option value="Hide">Hide Content</option>
                        <option value="Delete">Delete Content</option>
                        <option value="Ban">Ban User</option>
                    </select>
                    <input
                        type="text"
                        placeholder="Duration (e.g., 24h, 7d)"
                        value={moderationData.action.duration}
                        onChange={(e) => setModerationData({
                            ...moderationData,
                            action: { ...moderationData.action, duration: e.target.value }
                        })}
                        className="border p-1"
                    />
                </div>
                <textarea
                    placeholder="Reason"
                    value={moderationData.action.reason}
                    onChange={(e) => setModerationData({
                        ...moderationData,
                        action: { ...moderationData.action, reason: e.target.value }
                    })}
                    className="border p-1 w-full mb-2"
                />
                <textarea
                    placeholder="Moderator Notes"
                    value={moderationData.moderatorNotes}
                    onChange={(e) => setModerationData({ ...moderationData, moderatorNotes: e.target.value })}
                    className="border p-1 w-full mb-2"
                />
                <button
                    onClick={() => handleApi('/moderation/moderate', 'POST', moderationData)}
                    className="bg-yellow-500 text-white px-2 py-1"
                >
                    Moderate Content
                </button>
            </div>

            {/* Resolve Report */}
            <div className="mb-6">
                <h3 className="font-bold mb-2">Resolve Report</h3>
                <div className="mb-2">
                    <button onClick={() => copyMockData(mockResolveReport)} className="mr-2 bg-gray-200 px-2 py-1">
                        Copy Mock Resolution
                    </button>
                </div>
                <input
                    type="text"
                    placeholder="Report ID"
                    value={reportId}
                    onChange={(e) => setReportId(e.target.value)}
                    className="border p-1 mb-2 w-full"
                />
                <div className="grid grid-cols-2 gap-2 mb-2">
                    <select
                        value={resolveData.resolution}
                        onChange={(e) => setResolveData({ ...resolveData, resolution: e.target.value })}
                        className="border p-1"
                    >
                        <option value="">Select Resolution</option>
                        <option value="warned">Warned</option>
                        <option value="content_removed">Content Removed</option>
                        <option value="invalid">Invalid Report</option>
                        <option value="no_action">No Action Needed</option>
                    </select>
                    <select
                        value={resolveData.action.actionType}
                        onChange={(e) => setResolveData({
                            ...resolveData,
                            action: { ...resolveData.action, actionType: e.target.value }
                        })}
                        className="border p-1"
                    >
                        <option value="">Select Action</option>
                        <option value="Warning">Warning</option>
                        <option value="Hide">Hide Content</option>
                        <option value="Delete">Delete Content</option>
                        <option value="Ban">Ban User</option>
                    </select>
                </div>
                <textarea
                    placeholder="Resolution Notes"
                    value={resolveData.notes}
                    onChange={(e) => setResolveData({ ...resolveData, notes: e.target.value })}
                    className="border p-1 w-full mb-2"
                />
                <button
                    onClick={() => handleApi(`/moderation/reports/${reportId}/resolve`, 'PUT', resolveData)}
                    className="bg-green-500 text-white px-2 py-1"
                >
                    Resolve Report
                </button>
            </div>

            {/* Get Reports */}
            <div className="mb-6">
                <h3 className="font-bold mb-2">Get Reports</h3>
                <div className="mb-2">
                    <button
                        onClick={() => handleApi('/moderation/reports?status=pending', 'GET')}
                        className="bg-blue-500 text-white px-2 py-1 mr-2"
                    >
                        Pending Reports
                    </button>
                    <button
                        onClick={() => handleApi('/moderation/reports?status=resolved', 'GET')}
                        className="bg-blue-500 text-white px-2 py-1"
                    >
                        Resolved Reports
                    </button>
                </div>
            </div>

            {/* Get Moderation Stats */}
            <div className="mb-6">
                <h3 className="font-bold mb-2">Get Moderation Stats</h3>
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
                    onClick={() => handleApi(`/moderation/reports/stats?timeFrame=${timeFrame}`, 'GET')}
                    className="bg-blue-500 text-white px-2 py-1"
                >
                    Get Stats
                </button>
            </div>

            {/* Get Moderation History */}
            <div className="mb-6">
                <h3 className="font-bold mb-2">Get Moderation History</h3>
                <div className="grid grid-cols-2 gap-2 mb-2">
                    <input
                        type="text"
                        placeholder="Content ID"
                        value={moderationData.contentId}
                        onChange={(e) => setModerationData({ ...moderationData, contentId: e.target.value })}
                        className="border p-1"
                    />
                    <select
                        value={moderationData.contentType}
                        onChange={(e) => setModerationData({ ...moderationData, contentType: e.target.value })}
                        className="border p-1"
                    >
                        <option value="Post">Post</option>
                        <option value="Comment">Comment</option>
                    </select>
                </div>
                <button
                    onClick={() => handleApi(
                        `/moderation/content/${moderationData.contentId}/history?contentType=${moderationData.contentType}`,
                        'GET'
                    )}
                    className="bg-blue-500 text-white px-2 py-1"
                >
                    Get History
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