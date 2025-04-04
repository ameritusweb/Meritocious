import React, { useState } from 'react';
import { AuthSection } from './components/AuthSection';
import { PostsSection } from './components/PostsSection';
import { CommentsSection } from './components/CommentsSection';
import { RemixSection } from './components/RemixSection';
import { TagsSection } from './components/TagsSection';
import { SubstackSection } from './components/SubstackSection';
import { ModerationSection } from './components/ModerationSection';
import { MeritSection } from './components/MeritSection';
import { DiscoverySection } from './components/DiscoverySection';
import { AnalyticsSection } from './components/AnalyticsSection';

function App() {
    const [token, setToken] = useState(localStorage.getItem('token'));
    const [currentUser, setCurrentUser] = useState(null);

    const handleLogin = (newToken, user) => {
        localStorage.setItem('token', newToken);
        setToken(newToken);
        setCurrentUser(user);
    };

    const handleLogout = () => {
        localStorage.removeItem('token');
        setToken(null);
        setCurrentUser(null);
    };

    if (!token) {
        return <AuthSection onLogin={handleLogin} />;
    }

    return (
        <div className="p-4">
            <div className="mb-4 flex justify-between items-center">
                <h1 className="text-xl">Meritocious Mock Client</h1>
                <div>
                    {currentUser?.email}
                    <button
                        onClick={handleLogout}
                        className="ml-4 bg-red-500 text-white px-2 py-1 rounded"
                    >
                        Logout
                    </button>
                </div>
            </div>

            <div className="grid grid-cols-1 gap-4">
                <PostsSection token={token} />
                <CommentsSection token={token} />
                <RemixSection token={token} />
                <TagsSection token={token} />
                <SubstackSection token={token} />
                <ModerationSection token={token} />
                <MeritSection token={token} />
                <DiscoverySection token={token} />
                <AnalyticsSection token={token} />
            </div>
        </div>
    );
}

export default App;