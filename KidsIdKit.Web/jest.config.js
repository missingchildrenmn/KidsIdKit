// jest.config.js
module.exports = {
    // Specify the root directory for tests
    roots: ['<rootDir>/tests'],

    // Transform ES6 and JSX syntax using babel-jest
    transform: {
        '^.+\\.jsx?$': 'babel-jest',
    },

    // Specify the test environment
    testEnvironment: 'jsdom',

    // Optional: If you need to ignore certain paths
    testPathIgnorePatterns: ['/node_modules/', '/wwwroot/'],

    // Optional: Mock static assets
    moduleNameMapper: {
        '\\.(css|less|sass|scss)$': 'identity-obj-proxy',
        '\\.(gif|ttf|eot|svg|png)$': '<rootDir>/__mocks__/fileMock.js',
    },

    // Optional: Coverage settings
    collectCoverage: true,
    coverageDirectory: 'coverage',
    coverageReporters: ['text', 'lcov'],

    // Optional: Clear mocks between tests
    clearMocks: true,
};
