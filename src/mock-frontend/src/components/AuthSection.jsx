import React, { useState } from 'react';

const API_BASE_URL = 'http://localhost:5000/api';

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