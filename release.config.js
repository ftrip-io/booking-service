module.exports = {
  branches: "master",
  repositoryUrl: "https://github.com/ftrip-io/booking-service",
  plugins: [
    "@semantic-release/commit-analyzer",
    "@semantic-release/release-notes-generator",
    "@semantic-release/github",
  ],
};
