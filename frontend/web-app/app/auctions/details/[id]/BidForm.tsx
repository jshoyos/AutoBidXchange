'use client';
import { placeBidForAuction } from "@/app/actions/auctionActions";
import { useBidStore } from "@/app/hooks/useBidStore";
import { numberWithComma } from "@/app/lib/numberWithComma";
import React from "react";
import { FieldValues, useForm } from "react-hook-form";
import toast from "react-hot-toast";

type Props = {
    auctionId: string;
    highBid: number;
}

export default function BidForm({ auctionId, highBid }: Props) {
    const { register, handleSubmit, reset } = useForm();
    const addBid = useBidStore(state => state.addBid);

    function onSubmit(data: FieldValues) {
        if (data.amount <= highBid) {
            reset();
            return toast.error(`Your bid must be higher than the current high bid of $${numberWithComma(highBid)}.`);
        }
        placeBidForAuction(auctionId, +data.amount).then(bid => {
            if (bid.error) {
                reset();
                throw bid.error;
            }
            addBid(bid);
            reset();
        }).catch((error) => {
            toast.error(error.message);
        });
    }

    return (
        <form onSubmit={handleSubmit(onSubmit)}
            className="flex items-center border-2 rounded-lg py2">
            <input
                type="number"
                {...register('amount')}
                className="input-custom"
                placeholder={`Enter your bid (minimum bid is $${(highBid + 1)})`}
            />
        </form>
    )
}