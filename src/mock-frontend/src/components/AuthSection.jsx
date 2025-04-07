import React, { useState, useEffect } from 'react';

const API_BASE_URL = 'https://localhost:7214/api';

const mockLoginData = {
    email: "test@example.com",
    password: "GuessablePassword123!"
};

const mockRegisterData = {
    email: "newuser@example.com",
    username: "newuser",
    password: "GuessablePassword123!"
};

export const AuthSection = ({ onLogin }) => {
    const [isLogin, setIsLogin] = useState(true);
    const [formData, setFormData] = useState({
        email: '',
        password: '',
        username: ''
    });
    const [error, setError] = useState('');
    const [response, setResponse] = useState(null);

    useEffect(() => {
        // Initialize Google Sign-In
        if (window.google) {
            window.google.accounts.id.initialize({
                client_id: '412285972501-27gidm7cq4lg1mst81t3j43mahosuid8.apps.googleusercontent.com',
                callback: handleGoogleResponse
            });

            window.google.accounts.id.renderButton(
                document.getElementById('google-signin-button'),
                { theme: 'outline', size: 'large', width: '100%', text: 'signin_with' }
            );
        }
    }, []);

    // Handle the response from Google
    const handleGoogleResponse = async (response) => {
        try {
            setError('');

            // The ID token is in response.credential
            const idToken = response.credential;

            // Send the token to your backend
            const apiResponse = await fetch(`${API_BASE_URL}/auth/google`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ idToken }),
            });

            const data = await apiResponse.json();
            setResponse(data);

            if (!apiResponse.ok) {
                throw new Error(data.message || 'Google authentication failed');
            }

            if (data.accessToken) {
                onLogin(data.accessToken, data.user);
            }
        } catch (err) {
            setError(err.message);
        }
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');
        setResponse(null);

        try {
            const endpoint = isLogin ? '/auth/login' : '/users/register';
            const response = await fetch(API_BASE_URL + endpoint, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(formData),
            });

            const data = await response.json();
            setResponse(data);

            if (!response.ok) {
                throw new Error(data.message || 'Authentication failed');
            }

            if (data.token) {
                onLogin(data.token, data.user);
            }
        } catch (err) {
            setError(err.message);
        }
    };

    const copyMockData = () => {
        const data = isLogin ? mockLoginData : mockRegisterData;
        navigator.clipboard.writeText(JSON.stringify(data, null, 2));
    };

    const fillMockData = () => {
        const data = isLogin ? mockLoginData : mockRegisterData;
        setFormData(data);
    };

    return (
        <div className="p-4">
            <div className="mb-4">
                <button
                    onClick={() => setIsLogin(true)}
                    className={`mr-2 px-2 py-1 ${isLogin ? 'bg-blue-500 text-white' : 'bg-gray-200'}`}
                >
                    Login
                </button>
                <button
                    onClick={() => setIsLogin(false)}
                    className={`px-2 py-1 ${!isLogin ? 'bg-blue-500 text-white' : 'bg-gray-200'}`}
                >
                    Register
                </button>
            </div>

            <div className="mb-4">
                <button onClick={copyMockData} className="mr-2 bg-gray-200 px-2 py-1">
                    Copy Mock Data
                </button>
                <button onClick={fillMockData} className="bg-gray-200 px-2 py-1">
                    Fill Mock Data
                </button>
            </div>

            <form onSubmit={handleSubmit} className="mb-4">
                <div className="mb-2">
                    <input
                        type="email"
                        placeholder="Email"
                        value={formData.email}
                        onChange={(e) => setFormData({ ...formData, email: e.target.value })}
                        className="border p-1 mr-2"
                    />
                </div>
                {!isLogin && (
                    <div className="mb-2">
                        <input
                            type="text"
                            placeholder="Username"
                            value={formData.username}
                            onChange={(e) => setFormData({ ...formData, username: e.target.value })}
                            className="border p-1 mr-2"
                        />
                    </div>
                )}
                <div className="mb-2">
                    <input
                        type="password"
                        placeholder="Password"
                        value={formData.password}
                        onChange={(e) => setFormData({ ...formData, password: e.target.value })}
                        className="border p-1 mr-2"
                    />
                </div>
                <button type="submit" className="bg-green-500 text-white px-2 py-1">
                    {isLogin ? 'Login' : 'Register'}
                </button>
            </form>

            <div className="mb-4 border-t pt-4">
                <div id="google-signin-button" className="mt-2"></div>
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