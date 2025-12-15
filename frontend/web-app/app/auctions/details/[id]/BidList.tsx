'use client';
import { getBidsForAuction } from "@/app/actions/auctionActions";
import Heading from "@/app/components/Heading";
import { useBidStore } from "@/app/hooks/useBidStore";
import { Auction, Bid } from "@/types";
import { User } from "next-auth"
import React, { useEffect, useState } from "react";
import toast from "react-hot-toast";
import BidItem from "./BidItem";
import { numberWithComma } from "@/app/lib/numberWithComma";
import EmptyFilter from "@/app/components/EmptyFilter";
import BidForm from "./BidForm";

type Props = {
    user: User | null;
    auction: Auction;
}

export default function BidList({ user, auction }: Props) {
    const [loading, setLoading] = useState(false);
    const bids = useBidStore((state) => state.bids);
    const setBids = useBidStore((state) => state.setBid);
    const open = useBidStore((state) => state.open);
    const setOpen = useBidStore((state) => state.setOpen);
    const openForBids = new Date(auction.auctionEnd) > new Date();

    const highBid = bids.reduce((max, bid) => bid.amount > max
        ? bid.bidStatus.includes('Accepted')
            ? bid.amount
            : max
        : max, 0);

    useEffect(() => {
        getBidsForAuction(auction.id)
            .then((res: any) => {
                if (res.error) {
                    throw res.error;
                }
                setBids(res as Bid[]);
                setLoading(false);
            }).catch((error) => {
                toast.error(error.message);
            }).finally(() => {
                setLoading(false);
            });
    }, [auction.id, setBids]);

    useEffect(() => {
        setOpen(openForBids);
    }, [openForBids, setOpen]);

    if (loading) {
        return <span>Loading bids...</span>
    }

    return (
        <div className="rounded-lg shadow-md">
            <div className="py-2 px-4 bg-white">
                <div className="sticky top-o bg-white p-w">
                    <Heading title={`Current high bid is $${numberWithComma(highBid)}`} subtitle='' />
                </div>
                <div className="overflow-auto h-[350px] flex flex-col-reverse px-2">
                    {bids.length === 0 ? (
                        <EmptyFilter title="No bids placed yet." subtitle="Please feel free to make a bid" />
                    ) : (
                        <>
                            {bids.map((bid) => (
                                <BidItem key={bid.id} bid={bid} />
                            ))}
                        </>
                    )
                    }
                </div>
                <div className="px-2 pb-2 text-gray-500">
                    {!open ? (
                        <div className="flex items-center justify-center p-2 text-lg font-semibold">
                            This auction has finished.
                        </div>
                    ) : !user ? (
                        <div className="flex items-center justify-center p-2 text-lg font-semibold">
                            Please log in to place a bid.
                        </div>
                    ) : user && user.username === auction.seller ? (
                        <div className="flex items-center justify-center p-2 text-lg font-semibold">
                            You cannot place a bid on your own auction.
                        </div>
                    ) : (
                        <BidForm auctionId={auction.id} highBid={highBid} />
                    )}

                </div>
            </div>
        </div>
    )
}