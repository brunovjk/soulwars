// SPDX-License-Identifier: Apache-2.0
pragma solidity ^0.8.0;

import "@thirdweb-dev/contracts/base/ERC721Base.sol";
import "base64-sol/base64.sol";

contract CastleToken is ERC721Base {
    // Mapping to store the tokens owned by each user
    mapping(address => uint256[]) public tokenPerUser;

    /**
     * Constructor
     * @param _name - the name of the token
     * @param _symbol - the symbol of the token
     */
    constructor(string memory _name, string memory _symbol)
        ERC721Base(_name, _symbol, msg.sender, 0)
    {}

    /**
     * Check the validity of the input string
     * @param _tokenURI - the string to be checked
     * @return - true if the input string is valid, false otherwise
     */
    function checkInput(string memory _tokenURI) public pure returns (bool) {
        bytes32 hash = keccak256(bytes(_tokenURI));
        return
            hash == keccak256(bytes("0")) ||
            hash == keccak256(bytes("1")) ||
            hash == keccak256(bytes("2")) ||
            hash == keccak256(bytes("3")) ||
            hash == keccak256(bytes("4"));
    }

    /**
     * Formats the token URI for a Castle Token
     *
     * @param wizardMaterial - the new token URI
     * @return - The formatted token URI as a string
     */
    function formatTokenURI(string memory wizardMaterial)
        public
        pure
        returns (string memory)
    {
        // Encoding the metadata into a JSON string
        return
            string(
                abi.encodePacked(
                    // Specifying the format of the encoded data
                    "data:application/json;base64,",
                    Base64.encode(
                        bytes(
                            abi.encodePacked(
                                '{ "name":"YouCastleToken", "description":"Store progress in SoulWars game", "attributes": { "image": "https://ipfs.io/ipfs/QmT6iN5PrQes13TRnM3YhtFE1heUoxVa67Jsj7Gm93JWLT?filename=wizard.png", "WizardMaterial":"',
                                wizardMaterial,
                                '" }'
                            )
                        )
                    )
                )
            );
    }

    /**
     * Set the token URI for a specific token ID
     * @param _tokenId - the ID of the token
     * @param _tokenURI - the new token URI
     */
    function _setTokenURI(uint256 _tokenId, string memory _tokenURI)
        internal
        override
    {
        require(checkInput(_tokenURI), "Token URI not valid");
        fullURI[_tokenId] = formatTokenURI(_tokenURI);
    }

    /**
     * Mint a new CastleToken to a user
     * @param _to - the address of the user to receive the new token
     */
    function mintTo(
        address _to,
        string memory /*_tokenURI*/
    ) public override {
        // Ensure that the sender can only own a maximum of 3 tokens
        require(
            tokenPerUser[msg.sender].length <= 3,
            "Max 3 CastleToken per wallet"
        );

        // Get the next token ID to mint
        uint256 _tokenId = nextTokenIdToMint();
        // Add the token ID to the sender's token list
        tokenPerUser[msg.sender].push(_tokenId);

        // Set the token URI to 0
        _setTokenURI(_tokenId, "0");
        // Mint the token to the user
        _safeMint(_to, 1, "");
    }

    // Function to burn a token
    function burn(uint256 _tokenId) external override {
        // Loop through the token array
        for (uint256 i = 1; i < tokenPerUser[msg.sender].length; i++) {
            // If the token ID matches the one to be burned
            if (tokenPerUser[msg.sender][i] == _tokenId) {
                // Shift the remaining tokens in the array to fill the gap
                for (
                    uint256 j = i;
                    j < tokenPerUser[msg.sender].length - 1;
                    j++
                ) {
                    tokenPerUser[msg.sender][j] = tokenPerUser[msg.sender][
                        j + 1
                    ];
                }
                // Remove the last token in the array
                tokenPerUser[msg.sender].pop();
            }
        }
        // Burn the token
        _burn(_tokenId, true);
    }
}
