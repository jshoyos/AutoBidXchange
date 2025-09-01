import { create } from "zustand"

type State = {
    pageNumber: number
    pageSize: number
    pageCount: number
    searchTerm: string
    searchValue: string
    orderBy: string,
    FilterBy: string
    seller?: string
    winner?: string
}

type Actions = {
    setParams: (params: Partial<State>) => void
    reset: () => void
    setSearchValue: (value: string) => void
}

const initialStates: State = {
    pageNumber: 1,
    pageSize: 12,
    pageCount: 1,
    searchTerm: '',
    searchValue: '',
    orderBy: 'make',
    FilterBy: 'live',
    seller: undefined,
    winner: undefined
}

export const useParamsStore = create<State & Actions>()((set) => ({
    ...initialStates,

    setParams: (newParams: Partial<State>) => {
        set((state) => {
            if (newParams.pageNumber) {
                return {...state, pageNumber: newParams.pageNumber}
            } else {
                return {...state, ...newParams, pageNumber: 1}
            }
        })
    },
    
    reset: () => set(initialStates),

    setSearchValue: (value: string) => {
        set({searchValue: value})
    }
}))