'use client'
import React from 'react'
import { FaSearch } from 'react-icons/fa'
import { useParamsStore } from '../hooks/useParamsStore'

export default function Search() {
    const setParams = useParamsStore(state => state.setParams);
    const setSearchValue = useParamsStore(state => state.setSearchValue);
    const searchValue = useParamsStore(state => state.searchValue);

    function onChange(event: any) {
        setSearchValue(event.target.value);
    }

    function search() {
        setParams({searchTerm: searchValue});
    }

  return (
    <div className='flex w-[50%] items-center border-2 rounded-full py-2 shadow-sm'>
        <input
            type='text'
            placeholder='Search for cars by make, model, color'
            className='
                flex-grow
                pl-5
                bg-transparent
                border-none
                focus:border-transparent
                focus:ring-0
                focus:outline-none
                text-sm
                text-gray-600
            '
            onChange={onChange}
            onKeyDown={(e: any) => {
                if (e.key === 'Enter') search();
            }}
            value={searchValue}
        />
        <button onClick={search}>
            <FaSearch size={35} className='bg-red-400 text-white rounded-full p-2 cursor-pointer mx-2'/>
        </button>
    </div>
  )
}
