// SPDX-License-Identifier: Apache-2.0
pragma solidity ^0.8.0;

import "@thirdweb-dev/contracts/base/ERC1155LazyMint.sol";

contract ProfileToken is ERC1155LazyMint {
    // Mapping to store if a user has a token or not
    mapping(address => bool) tokenPerUser;

    /**
     * Constructor
     * @param _name - the name of the token
     * @param _symbol - the symbol of the token
     */
    constructor(string memory _name, string memory _symbol)
        ERC1155LazyMint(_name, _symbol, msg.sender, 0)
    {}

    /**
     * Verifies the claim of a token
     * @param _claimer - the address claiming the token
     * @param _tokenId - the id of the token being claimed
     * @param _quantity - the number of tokens being claimed
     */
    function verifyClaim(
        address _claimer,
        uint256 _tokenId,
        uint256 _quantity
    ) public view override {
        // Check that the user has not already claimed a token
        require(!tokenPerUser[_claimer], "One ProfileToken per wallet");
        // Check that the token being claimed is the profile token (id = 0)
        require(_tokenId == 0, "Only ProfileToken");
        // Check that the user is claiming only one token
        require(_quantity == 1, "Only one token");
    }

    /**
     * Transfers the token to the user on successful claim
     * @param _receiver - the address to receive the token
     */
    function _transferTokensOnClaim(
        address _receiver,
        uint256, /*_tokenId*/
        uint256 /*_quantity*/
    ) internal override {
        // Set the user as having claimed a token
        tokenPerUser[_receiver] = true;
        // Mint the token to the user
        _mint(_receiver, 0, 1, "");
    }

    /**
     * Burns the token for the user
     * @param _owner - the owner of the token
     * @param _amount - the number of tokens to burn
     */
    function burn(
        address _owner,
        uint256, /*_tokenId*/
        uint256 _amount
    ) external override {
        // Get the caller's address
        address caller = msg.sender;
        // Set the user as no longer having a token
        tokenPerUser[_owner] = false;

        // Check that the caller is the owner or is approved for the owner
        require(
            caller == _owner || isApprovedForAll[_owner][caller],
            "Unapproved caller"
        );
        // Check that the owner has enough tokens to burn
        require(balanceOf[_owner][0] >= _amount, "Not enough tokens owned");

        // Burn the tokens
        _burn(_owner, 0, _amount);
    }
}
