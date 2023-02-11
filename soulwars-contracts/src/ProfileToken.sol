// SPDX-License-Identifier: Apache-2.0
pragma solidity ^0.8.0;

import "@thirdweb-dev/contracts/base/ERC1155LazyMint.sol";

contract ProfileToken is ERC1155LazyMint {
    mapping(address => bool) tokenPerUser;

    constructor(string memory _name, string memory _symbol)
        ERC1155LazyMint(_name, _symbol, msg.sender, 0)
    {}

    function verifyClaim(
        address _claimer,
        uint256 _tokenId,
        uint256 _quantity
    ) public view override {
        require(!tokenPerUser[_claimer], "One ProfileToken per wallet");
        require(_tokenId == 0, "Only ProfileToken");
        require(_quantity == 1, "Only one token");
    }

    function _transferTokensOnClaim(
        address _receiver,
        uint256, /*_tokenId*/
        uint256 /*_quantity*/
    ) internal override {
        tokenPerUser[_receiver] = true;
        _mint(_receiver, 0, 1, "");
    }

    function burn(
        address _owner,
        uint256, /*_tokenId*/
        uint256 _amount
    ) external override {
        address caller = msg.sender;
        tokenPerUser[_owner] = false;

        require(
            caller == _owner || isApprovedForAll[_owner][caller],
            "Unapproved caller"
        );
        require(balanceOf[_owner][0] >= _amount, "Not enough tokens owned");

        _burn(_owner, 0, _amount);
    }
}
